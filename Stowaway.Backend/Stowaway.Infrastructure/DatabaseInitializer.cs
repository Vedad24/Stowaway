using Stowaway.Infrastructure.Database;
using Stowaway.Infrastructure.Database.Seeders;
using Stowaway.Shared.Constants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Stowaway.Infrastructure;

public static class DatabaseInitializer
{
    /// <summary>
    /// Centralized migration and seeding.
    /// </summary>
    public static async Task InitializeDatabaseAsync(this IServiceProvider services, IHostEnvironment env)
    {
        await using var scope = services.CreateAsyncScope();
        var ctx = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        if (env.IsTest())
        {
            await ctx.Database.EnsureCreatedAsync();
            // resetIdentitySeeds: false — IsTest() runs against the InMemory provider,
            // which doesn't support ExecuteSqlRawAsync/DBCC CHECKIDENT, and each test DB
            // starts empty anyway so there's no ID-gap issue.
            await StaticDataSeeder.SeedAsync(ctx, resetIdentitySeeds: false);
            await DynamicDataSeeder.SeedAsync(ctx);
            return;
        }

        // SQL Server or similar
        await ctx.Database.MigrateAsync();

        await StaticDataSeeder.SeedAsync(ctx, resetIdentitySeeds: false);

        if (env.IsDevelopment())
        {
            await DynamicDataSeeder.SeedAsync(ctx);
        }
    }
}