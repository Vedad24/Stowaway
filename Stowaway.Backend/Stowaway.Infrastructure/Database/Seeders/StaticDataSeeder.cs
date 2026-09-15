using Stowaway.Shared.Constants;
using Serilog;
using Stowaway.Domain.Entities.Identity;
using Stowaway.Domain.Entities.Sales;
using Stowaway.Domain.Entities.Storage;
using Stowaway.Domain.Entities.Storage.StorageIdentity;

namespace Stowaway.Infrastructure.Database.Seeders;

/// <summary>
/// Seeder for lookup/reference data the app depends on (permissions, roles,
/// order/container statuses, container types). Runs on every app startup, in
/// every environment. Reconciles (add-missing) rather than wiping the DB.
/// </summary>
public static class StaticDataSeeder
{
    public static async Task SeedAsync(DatabaseContext context, bool resetIdentitySeeds)
    {
        if (resetIdentitySeeds) //Only good for one-time resetting, like after adding or removing priviledges after that there is no need.
        {
            await ResetPermissionsAsync(context);
            await ResetPriviledgesAsync(context);
        }

        await SeedPermissionsAsync(context);
        await SeedPrivilegesAsync(context);
        await SeedRolesAsync(context);
        await SeedRolePermissionsAsync(context);
        await SeedOrderStatusAsync(context);
        await SeedContainerTypesAsync(context);
        await SeedContainerStatusesAsync(context);
    }

    private static async Task ResetPermissionsAsync(DatabaseContext context)
    {
        // Permission_Role FKs into Permissions; clear it first.
        await context.Database.ExecuteSqlRawAsync("DELETE FROM [Identity].[Permission_Role]");
        await context.Database.ExecuteSqlRawAsync("DELETE FROM [Identity].[Permission]");
        await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('[Identity].[Permission]', RESEED, 0)");
    }

    private static async Task ResetPriviledgesAsync(DatabaseContext context)
    {
        
        // PriviledgeGroup_Priviledge FKs into Priviledges. DynamicDataSeeder.SeedStorageIdentityAsync
        // rebuilds these links idempotently — but only runs in Dev/Test, so this reset must only be
        // invoked with resetIdentitySeeds: true from those same environments (see DatabaseInitializer).
        await context.Database.ExecuteSqlRawAsync("DELETE FROM [StorageIdentity].[PriviledgeGroup_Priviledge]");
        await context.Database.ExecuteSqlRawAsync("DELETE FROM [StorageIdentity].[Priviledge]");
        await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('[StorageIdentity].[Priviledge]', RESEED, 0)");
    }

    private static async Task SeedPermissionsAsync(DatabaseContext context)
    {
        var permissionDescriptions = new[]
        {
            Permissions.UsersRead,
            Permissions.UsersCreate,
            Permissions.UsersUpdate,
            Permissions.UsersDelete,
            Permissions.UsersSelfRead,
            Permissions.UsersSelfUpdate,
            Permissions.UsersSelfDelete,
            Permissions.RolesRead,
            Permissions.RolesCreate,
            Permissions.RolesUpdate,
            Permissions.RolesDelete,
            Permissions.WarehouseCreate,
            Permissions.WarehouseRead,
            Permissions.WarehouseUpdate,
            Permissions.WarehouseDelete,
            Permissions.ContainerRead,
            Permissions.ContainerCreate,
            Permissions.ContainerUpdate,
            Permissions.ContainerDelete,
            Permissions.ItemRead,
            Permissions.ItemCreate,
            Permissions.ItemUpdate,
            Permissions.ItemDelete,
            Permissions.TagRead,
            Permissions.TagCreate,
            Permissions.OrderRead,
            Permissions.OrderCreate,
            Permissions.OrderUpdate,
            Permissions.OrderDelete,
            Permissions.SupplierRead,
            Permissions.SupplierCreate,
            Permissions.SupplierUpdate,
            Permissions.SupplierDelete,
            Permissions.CartManage,
            Permissions.CartManageAny,
            Permissions.WarehouseUsersManage,
            Permissions.OrderCreateAny
        };

        var existingDescriptions = await context.Permissions
            .Where(p => permissionDescriptions.Contains(p.Description))
            .Select(p => p.Description)
            .ToListAsync();

        var missingPermissions = permissionDescriptions
            .Where(d => !existingDescriptions.Contains(d))
            .Select(d => new PermissionEntity { Description = d })
            .ToList();

        if (missingPermissions.Count == 0)
        {
            Log.Information("✅ Static seed: permissions already exist.");
            return;
        }

        context.Permissions.AddRange(missingPermissions);
        await context.SaveChangesAsync();
        Log.Information("✅ Static seed: {Count} permissions added.", missingPermissions.Count);
    }

    private static async Task SeedPrivilegesAsync(DatabaseContext context)
    {
        var privilegeNames = Priviledges.AllCodes;

        var existingPrivileges = await context.Priviledges
            .Where(permission => privilegeNames.Contains(permission.Code))
            .Select(permission => permission.Code)
            .ToListAsync();

        var missingPrivileges = privilegeNames
            .Where(name => !existingPrivileges.Contains(name))
            .Select((name, index) => new PriviledgeEntity
            {
                Code = name,
                Description = name
            })
            .ToList();

        if (missingPrivileges.Count == 0)
        {
            Log.Information("✅ Static seed: warehouse privileges already exist.");
            return;
        }

        context.Priviledges.AddRange(missingPrivileges);

        await context.SaveChangesAsync();
        Log.Information("✅ Static seed: {Count} warehouse privileges added.", missingPrivileges.Count);
    }

    private static async Task SeedRolesAsync(DatabaseContext context)
    {
        var requiredRoles = new[]
        {
            Role.Admin,
            Role.User,
            Role.Manager
        };

        var existingRoleIds = await context.Roles
            .Where(r => requiredRoles.Contains(r.Id))
            .Select(r => r.Id)
            .ToListAsync();

        var missingRoles = requiredRoles
            .Where(roleId => !existingRoleIds.Contains(roleId))
            .Select(roleId => new RoleEntity
            {
                Id = roleId
            })
            .ToList();

        if (missingRoles.Count == 0)
        {
            Log.Information("✅ Static seed: roles already exist.");
            return;
        }

        context.Roles.AddRange(missingRoles);
        await context.SaveChangesAsync();

        Log.Information("✅ Static seed: {Count} roles added.", missingRoles.Count);
    }

    private static async Task SeedOrderStatusAsync(DatabaseContext context)
    {
        if (await context.OrderStatuses.AnyAsync())
            return;
        var OrderStatusNames = Enum.GetNames(typeof(OrderStatus));
        for (var i = 1; i <= OrderStatusNames.Length; i++)
        {
            OrderStatusEntity status = new OrderStatusEntity()
            {
                Id = (OrderStatus)i,
                Description = OrderStatusNames[i - 1]
            };
            context.OrderStatuses.Add(status);
            context.SaveChanges();
        }
        Log.Information("✅ Static seed: order statuses added.");
    }

    private static async Task SeedContainerTypesAsync(DatabaseContext context)
    {
        var existing = await context.ContainerTypes.ToListAsync();

        void EnsureType(int maxContainers, int maxItems, decimal price)
        {
            if (existing.Any(t => t.MaxContainers == maxContainers && t.MaxItems == maxItems))
            {
                return;
            }

            context.ContainerTypes.Add(new ContainerTypeEntity
            {
                MaxContainers = maxContainers,
                MaxItems = maxItems,
                Price = price,
            });
        }

        // Tiny holds items only — it can't hold sub-containers at all (MaxContainers = 0),
        // which makes it the floor of the size hierarchy: nothing can nest inside it.
        EnsureType(maxContainers: 0, maxItems: 10, price: 0.5m);
        EnsureType(maxContainers: 5, maxItems: 50, price: 1m);
        EnsureType(maxContainers: 10, maxItems: 100, price: 2m);
        EnsureType(maxContainers: 20, maxItems: 500, price: 3m);

        if (context.ChangeTracker.HasChanges())
        {
            await context.SaveChangesAsync();
            Log.Information("✅ Static seed: container types added.");
        }
    }

    private static async Task SeedContainerStatusesAsync(DatabaseContext context)
    {
        if (await context.ContainerStatuses.AnyAsync())
        {
            return;
        }

        var statuses = new List<ContainerStatusEntity>
        {
            new() { Description = "Active" },
            new() { Description = "Full" },
            new() { Description = "Under maintenance" },
            new() { Description = "Needs inspection" },
            new() { Description = "Damaged" },
            new() { Description = "Incoming" },
            new() { Description = "Outgoing" },
        };

        context.ContainerStatuses.AddRange(statuses);
        await context.SaveChangesAsync();
        Log.Information("✅ Static seed: container statuses added.");
    }

    // Explicit per-role permission matrix. Admin gets every permission that exists.
    // Manager gets everything except Roles.Create/Update/Delete.
    // User only gets its own module access (Self, Warehouse.Read, Container/Item/Tag/Supplier.Read).
    // Users/Employees have no Cart or Order access at all - only Manager and Admin do.
    private static readonly string[] ManagerOnlyPermissions =
    {
        Permissions.UsersRead, Permissions.UsersCreate, Permissions.UsersUpdate, Permissions.UsersDelete,
        Permissions.RolesRead,
        Permissions.WarehouseCreate, Permissions.WarehouseUpdate, Permissions.WarehouseDelete,
        Permissions.OrderRead, Permissions.OrderCreate,
        Permissions.SupplierCreate, Permissions.SupplierUpdate, Permissions.SupplierDelete,
        Permissions.WarehouseUsersManage,
        Permissions.CartManage
    };

    private static readonly string[] AdminOnlyPermissions =
    {
        Permissions.RolesCreate, Permissions.RolesUpdate, Permissions.RolesDelete,
        Permissions.OrderUpdate, Permissions.OrderDelete, Permissions.OrderCreateAny,
        Permissions.CartManage,
        Permissions.CartManageAny

    };

    private static readonly string[] SharedByAllRolesPermissions =
    {
        Permissions.UsersSelfRead, Permissions.UsersSelfUpdate, Permissions.UsersSelfDelete,
        Permissions.WarehouseRead,
        Permissions.ContainerRead, Permissions.ContainerCreate, Permissions.ContainerUpdate, Permissions.ContainerDelete,
        Permissions.ItemRead, Permissions.ItemCreate, Permissions.ItemUpdate, Permissions.ItemDelete,
        Permissions.TagRead, Permissions.TagCreate,
        Permissions.SupplierRead,
    };

    private static async Task SeedRolePermissionsAsync(DatabaseContext context)
    {
        if (!await context.Permissions.AnyAsync())
            return;

        var permissions = await context.Permissions
            .AsNoTracking()
            .ToListAsync();

        var userPermissions = new HashSet<string>(SharedByAllRolesPermissions, StringComparer.OrdinalIgnoreCase);
        var managerPermissions = new HashSet<string>(SharedByAllRolesPermissions, StringComparer.OrdinalIgnoreCase);
        managerPermissions.UnionWith(ManagerOnlyPermissions);
        var adminPermissions = new HashSet<string>(managerPermissions, StringComparer.OrdinalIgnoreCase);
        adminPermissions.UnionWith(AdminOnlyPermissions);

        var desiredByRole = new Dictionary<Role, HashSet<string>>
        {
            [Role.User] = userPermissions,
            [Role.Manager] = managerPermissions,
            [Role.Admin] = adminPermissions,
        };

        var existingAssignments = await context.PermissionRoles
            .Include(permissionRole => permissionRole.Permission)
            .ToListAsync();

        var toAdd = new List<Permission_RoleEntity>();

        foreach (var (role, desiredPermissions) in desiredByRole)
        {
            var alreadyGranted = existingAssignments
                .Where(a => a.RoleId == role)
                .Select(a => a.Permission.Description)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var permission in permissions.Where(p => desiredPermissions.Contains(p.Description) && !alreadyGranted.Contains(p.Description)))
            {
                toAdd.Add(new Permission_RoleEntity { RoleId = role, PermissionId = permission.Id });
            }
        }

        // Reconcile: drop any previously-granted row that the current matrix no longer grants.
        var toRemove = existingAssignments
            .Where(a => !desiredByRole.TryGetValue(a.RoleId, out var desired) || !desired.Contains(a.Permission.Description))
            .ToList();

        if (toRemove.Count > 0)
        {
            context.PermissionRoles.RemoveRange(toRemove);
        }

        if (toAdd.Count > 0)
        {
            context.PermissionRoles.AddRange(toAdd);
        }

        if (toAdd.Count == 0 && toRemove.Count == 0)
        {
            Log.Information("✅ Static seed: role permissions already up to date.");
            return;
        }

        await context.SaveChangesAsync();

        Log.Information("✅ Static seed: {AddedCount} role permissions added, {RemovedCount} stale role permissions removed.", toAdd.Count, toRemove.Count);
    }
}
