namespace Market.Infrastructure.Database.Seeders;

/// <summary>
/// Dynamic seeder koji se pokreće u runtime-u,
/// obično pri startu aplikacije (npr. u Program.cs).
/// Koristi se za unos demo/test podataka koji nisu dio migracije.
/// </summary>
public static class DynamicDataSeeder
{
    public static async Task SeedAsync(DatabaseContext context)
    {
        // Osiguraj da baza postoji (bez migracija)
        await context.Database.EnsureCreatedAsync();

        //await SeedProductCategoriesAsync(context);
        await SeedUsersAsync(context);
    }

    //private static async Task SeedProductCategoriesAsync(DatabaseContext context)
    //{
    //    if (!await context.Warehouses.AnyAsync())
    //    {
    //        context.Warehouses.AddRange(
    //            new WarehouseEntity
    //            {
    //                Name = "Računari (demo)",
    //                IsEnabled = true,
    //                CreatedAtUtc = DateTime.UtcNow
    //            },
    //            new WarehouseEntity
    //            {
    //                Name = "Mobilni uređaji (demo)",
    //                IsEnabled = true,
    //                CreatedAtUtc = DateTime.UtcNow
    //            }
    //        );

    //        await context.SaveChangesAsync();
    //        Console.WriteLine("✅ Dynamic seed: product categories added.");
    //    }
    //}

    /// <summary>
    /// Kreira demo korisnike ako ih još nema u bazi.
    /// </summary>
    private static async Task SeedUsersAsync(DatabaseContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        var hasher = new PasswordHasher<UserEntity>();

        var admin = new UserEntity
        {
            Email = "admin@market.local",
            PasswordHash = hasher.HashPassword(null!, "Admin123!"),
            IsEnabled = true,
        };

        var user = new UserEntity
        {
            Email = "manager@market.local",
            PasswordHash = hasher.HashPassword(null!, "User123!"),
            
            IsEnabled = true,
        };

        var dummyForSwagger = new UserEntity
        {
            Email = "string",
            PasswordHash = hasher.HashPassword(null!, "string"),
            
            IsEnabled = true,
        };
        var dummyForTests = new UserEntity
        {
            Email = "test",
            PasswordHash = hasher.HashPassword(null!, "test123"),
            
            IsEnabled = true,
        };
        context.Users.AddRange(admin, user, dummyForSwagger, dummyForTests);
        await context.SaveChangesAsync();

        Console.WriteLine("✅ Dynamic seed: demo users added.");
    }
}