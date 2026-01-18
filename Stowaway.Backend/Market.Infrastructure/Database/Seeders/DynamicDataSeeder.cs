using Stowaway.Domain.Entities.Identity;
using Stowaway.Domain.Entities.Sales;
using Stowaway.Domain.Entities.Storage;
using System.Numerics;

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
        await SeedSupplierAsync(context);
        await SeedWarehouseAsync(context);
        await SeedContainersAsync(context);
        await SeedItemsAsync(context);
        await SeedOrdersAsync(context);

    }

    private static async Task SeedOrdersAsync(DatabaseContext context)
    {
        if (await context.Orders.AnyAsync())
            return;
        var order = new OrderEntity
        {
            OrderDate = DateTime.Now,
            Subtotal = 0,
            Total = 0,
            OrderStatusId = OrderStatus.Draft,
            User = context.Users.FirstOrDefault(),
        };
        context.Orders.Add(order);

        ContainerTypeEntity? containerTypeEntity = context.ContainerTypes.FirstOrDefault();
        var orderItems = new List<OrderItemEntity>
            {
                new OrderItemEntity
                {
                    Order = order,
                    ContainerType = containerTypeEntity,
                    Discount = 0.05m,
                    Subtotal = containerTypeEntity.Price * 10,
                    Total = containerTypeEntity.Price * (1m-0.05m) * 10,
                    Quantity = 10,
                    UnitPrice = containerTypeEntity.Price,
                }
            };
        order.Subtotal = orderItems.Sum(oi => oi.Subtotal);
        order.Total = orderItems.Sum(oi => oi.Total);
        order.orderItems = orderItems;
        context.SaveChanges();
        Console.WriteLine("✅ Dynamic seed: demo orders added.");

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

    private static async Task SeedSupplierAsync(DatabaseContext context)
    {
        if(await context.Suppliers.AnyAsync()){
            return;  
        }
        var CocktaSupplier = new SupplierEntity
        {
            Name = "Cockta",
            Description = "Soda company",
            Address = "Ulica Kralja Tomislava, Mostar",
            TotalDeliveries = 10,
            FailedDeliveries = 0,
        };
        var OazaSupplier = new SupplierEntity
        {
            Name = "Oaza",
            Address = "Karadaglije bb, Tešanj",
            Description = "Water company",
            TotalDeliveries = 50,
            FailedDeliveries = 3,
        };

        context.Suppliers.AddRange(CocktaSupplier, OazaSupplier);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Dynamic seed: demo suppliers added.");
    }

    private static async Task SeedWarehouseAsync(DatabaseContext context)
    {
        if (await context.Warehouses.AnyAsync())
        {
            return;
        }

        var MainStorage = new WarehouseEntity
        {
            Name = "Main storage room",
            Description = "Big containers only.",
            City = "Mostar",
            Address = "Brace Fejica 30",
            Capacity = 40,
            isEnabled = true,
        };

        var SmallRoom = new WarehouseEntity
        {
            Name = "Small room",
            Description = "Water only",
            City = "Mostar",
            Address = "Brace Fejica 30",
            Capacity = 5,
            isEnabled = true,
        };

        context.Warehouses.AddRange(MainStorage, SmallRoom);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Dynamic seed: demo warehouses added.");
    }

    private static async Task SeedContainersAsync(DatabaseContext context)
    {
        if (await context.Containers.AnyAsync())
        {
            return;
        }

        var CardboardBox = new ContainerEntity
        {
            Name = "Cardboard box",
            ContainerTypeId = 1,
            ParentContainerId = null,
            WarehouseId = 2
        };

        var WoodenPallet = new ContainerEntity
        {
            Name = "Wooden pallet",
            ContainerTypeId = 2,
            ParentContainerId = null,
            WarehouseId = 1
        };

        context.Containers.AddRange(CardboardBox, WoodenPallet);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Dynamic seed: demo containers added.");
    }

    private static async Task SeedItemsAsync(DatabaseContext context)
    {
        if (await context.Item.AnyAsync())
        {
            return;
        }

        var WaterBottle = new ItemEntity
        {
            Name = "Natural water",
            Description = "0,5l plastic bottle",
            Quantity = 50,
            ContainerId = 1,
            SupplierId = 2,
            ByteImage = null,
        };

        var CocktaBottle = new ItemEntity
        {
            Name = "Cockta soda",
            Description = "0,5l plastic bottle",
            Quantity = 85,
            ContainerId = 2,
            SupplierId = 1,
            ByteImage = null,
        };

        context.Item.AddRange(WaterBottle, CocktaBottle);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Dynamic seed: demo items added.");
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