using Stowaway.Domain.Entities.Identity;
using Stowaway.Domain.Entities.Sales;
using Stowaway.Domain.Entities.Storage;

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
        await SeedContainerTypesAsync(context);
        await SeedOrderStatusAsync(context);
        await SeedRolesAsync(context);
        await SeedUsersAsync(context);
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
                Id = (OrderStatus) i,
                Description = OrderStatusNames[i - 1]
            };
            context.OrderStatuses.Add(status);
            //context.OrderStatuses.Add(new OrderStatusEntity { Description = OrderStatusNames[i-1] });
            context.SaveChanges();
        }
        //await context.SaveChangesAsync();
        Console.WriteLine("✅ Dynamic seed: demo order status added.");
    }

    private static async Task SeedContainerTypesAsync(DatabaseContext context)
    {
        if(await context.ContainerTypes.AnyAsync())
            return;
        var SmallContainer = new ContainerTypeEntity
        {
            MaxContainers = 5,
            MaxItems = 50,
            Price = 1m
        };
        var MediumContainer = new ContainerTypeEntity
        {
            MaxContainers = 10,
            MaxItems = 100,
            Price = 2m
        };
        var LargeContainer = new ContainerTypeEntity
        {
            MaxContainers = 20,
            MaxItems = 500,
            Price = 3m
        };
        context.ContainerTypes.AddRange(SmallContainer, MediumContainer, LargeContainer);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Dynamic seed: demo container types added.");
    }

    

    /// <summary>
    /// Kreira demo korisnike ako ih još nema u bazi.
    /// </summary>

    private static async Task SeedRolesAsync(DatabaseContext context)
    {
        if (await context.Roles.AnyAsync())
            return;
        var roleAdmin = new RoleEntity
        {
            Id = Role.Admin
        };
        var roleUser = new RoleEntity
        {
            Id = Role.User
        };
        context.Roles.AddRange(roleAdmin, roleUser);
        await context.SaveChangesAsync();


        Console.WriteLine("✅ Dynamic seed: demo roles added.");
    }
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