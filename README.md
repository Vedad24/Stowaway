# Stowaway

Multi-tenant warehouse/inventory management system with an ASP.NET Core backend, an Angular frontend, and Stripe-backed order checkout. Warehouses are the isolation boundary: users are assigned to warehouses via privilege groups that gate what they can see and do within that warehouse.

## Repository layout

| Path | What it is |
|---|---|
| [Stowaway.Backend/](Stowaway.Backend) | ASP.NET Core 9 Web API (Clean Architecture: API / Application / Domain / Infrastructure / Shared / Tests) |
| [Stowaway.Frontend/](Stowaway.Frontend) | Angular 21 SPA (standalone components, Angular Material, SSR-capable) |
| [Stowaway.Support/](Stowaway.Support) | Supporting design artifacts (Visual Paradigm project, ER diagrams)

## Architecture

The backend follows Clean Architecture, split into five projects under [Stowaway.Backend/](Stowaway.Backend):

- **Market.Domain** — entities and base types, no dependencies on other layers.
- **Market.Application** — CQRS use cases (MediatR commands/queries + handlers + FluentValidation validators), organized by bounded module under `Modules/`: `Auth`, `Identity`, `Sales`, `Storage`.
- **Market.Infrastructure** — EF Core `DatabaseContext`, entity configurations, migrations, database seeders, Stripe payment integration.
- **Market.API** — controllers, authentication/authorization (JWT + custom privilege attributes), middleware, DI wiring, `Program.cs`.
- **Market.Shared** — cross-cutting DTOs, constants (`Permissions`, `Priviledges`), and options classes shared by API/Application/Infrastructure.
- **Market.Tests** — integration/unit tests using `CustomWebApplicationFactory`.

See [Stowaway.Backend/layers.png](Stowaway.Backend/layers.png) for the layer diagram.

### Domain model

- **Identity**: `User`, `Role`, `Permission`, `Permission_Role`, `RefreshToken`.
- **Storage**: `Warehouse`, `Container`, `ContainerType`, `ContainerStatus`/`ContainerStatusHistory`, `Item`, `ItemImage`, `Item_Tag`, `Tag`, `Supplier`.
- **Storage identity** (per-warehouse access control): `Priviledge`, `PriviledgeGroup`, `PriviledgeGroup_Priviledge`, `Warehouse_User` — a user is assigned exactly one privilege group per warehouse, which determines their allowed actions in that warehouse.
- **Sales**: `Order`, `OrderItem`, `OrderStatus`, `CartItem`, `ProcessedStripeEvent`.

### Authorization model

Two layers stack on top of standard JWT bearer authentication:

1. **`[HasPermission(Permissions.X)]`** — coarse, role-based. A permission is granted to one or more `Role`s (`Admin`, `Manager`, `Employee`(User)) via the `StaticDataSeeder`.
2. **`[HasPriviledge(..., WarehouseResolutionStrategy)]`** — fine-grained, per-warehouse. Resolves which warehouse the request targets (from route id, container id, item id, etc.) and checks the caller's `Warehouse_User` → `PriviledgeGroup` assignment for that specific warehouse.

Most write endpoints combine both: a `[HasPermission]` role gate plus a `[HasPriviledge]` warehouse-scope gate.

## Prerequisites

- .NET 9 SDK
- Node.js 20+ and npm
- SQL Server (LocalDB or full instance) — default connection string targets `Server=localhost;Database=StowawayNew`
- A Stripe account/API keys if you want to exercise checkout (optional for browsing/inventory features)

## Running the backend

```bash
cd Stowaway.Backend
dotnet restore
dotnet run --project Market.API
```

On startup the app runs `ctx.Database.MigrateAsync()` and then seeds:

- **Static data** — roles, permissions, privileges, container/order statuses, container types (idempotent, runs in every environment).
- **Dynamic data** — demo warehouses, users, items, etc. (Development environment only).

This creates a default admin account:

```
email:    admin@market.local
password: Admin123!
```

> Change or remove this seeded account, and replace the placeholder `Jwt:Key` in `appsettings.json`, before deploying anywhere outside local development.

API docs (Swagger) are available at the app's root when running in Development.

Configuration lives in [Stowaway.Backend/Market.API/appsettings.json](Stowaway.Backend/Market.API/appsettings.json) (`ConnectionStrings:Main`, `Jwt`, `Cors:AllowedOrigins`, `Frontend:BaseUrl`, Serilog sinks). Override per-environment via `appsettings.Development.json`, environment variables, or user-secrets.

### Running tests

```bash
cd Stowaway.Backend
dotnet test
```

## Running the frontend

```bash
cd Stowaway.Frontend
npm install
npm start        # ng serve, http://localhost:4200
```

Other scripts (see [Stowaway.Frontend/package.json](Stowaway.Frontend/package.json)):

```bash
npm run build     # production build, output to dist/
npm test          # unit tests via Vitest
npm run watch     # incremental dev build
```

The frontend's API base URL is set in `src/enviroments/enivroment.ts` (`apiUrl`). There is currently only one environment file, so it applies to both dev and production builds — update it if your backend runs somewhere other than `http://localhost:5177`.

## Running the full stack locally

1. Start SQL Server and make sure the connection string in `appsettings.json` (or Development overrides) points at it.
2. `dotnet run --project Stowaway.Backend/Market.API` — API comes up (default `http://localhost:5177`), migrates + seeds the database automatically.
3. `npm start` inside `Stowaway.Frontend` — SPA comes up on `http://localhost:4200` and talks to the API via the configured `apiUrl`.
4. Log in with the seeded admin account above, or sign up a new user.

## Payments

Stripe checkout is proxied entirely through the backend (`StripePaymentController`, `Market.Infrastructure/Payments/Stripe`) — no Stripe key is ever exposed to the frontend. Configure `Stripe:ApiKey` (and, once wired, `Stripe:WebhookSecret`) via user-secrets or environment variables; do not commit real keys to `appsettings.json`.

