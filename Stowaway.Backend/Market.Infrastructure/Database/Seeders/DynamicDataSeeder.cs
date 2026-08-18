using Market.Shared.Constants;
using Serilog;
using Stowaway.Domain.Entities.Identity;
using Stowaway.Domain.Entities.Sales;
using Stowaway.Domain.Entities.Storage;
using Stowaway.Domain.Entities.Storage.StorageIdentity;
using System.Numerics;

namespace Market.Infrastructure.Database.Seeders;

/// <summary>
/// Dynamic seeder that runs at runtime,
/// usually on application startup (e.g. in Program.cs).
/// Used to insert demo/test data that isn't part of a migration.
/// </summary>
public static class DynamicDataSeeder
{
    public static async Task SeedAsync(DatabaseContext context)
    {
        // Ensure the database exists (without migrations)
        await context.Database.EnsureCreatedAsync();
        await SeedUsersAsync(context);
        await SeedSupplierAsync(context);
        await SeedWarehouseAsync(context);
        await SeedContainersAsync(context);
        await SeedContainerStatusHistoryAsync(context);
        await SeedItemsAsync(context);
        await SeedTagsAsync(context);
        await SeedItemTagsAsync(context);
        await SeedOrdersAsync(context);
        await SeedStorageIdentityAsync(context);
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
        var exampleWarehouse = context.Warehouses.First().Id;
        ContainerTypeEntity? containerTypeEntity = context.ContainerTypes.FirstOrDefault();
        var orderItems = new List<OrderItemEntity>
            {
                new OrderItemEntity
                {
                    Order = order,
                    ContainerType = containerTypeEntity,
                    WarehouseId = exampleWarehouse,
                    Discount = 0.05m,
                    Subtotal = containerTypeEntity.Price * 10,
                    Total = containerTypeEntity.Price * (1m-0.05m) * 10,
                    Quantity = 10,
                    UnitPrice = containerTypeEntity.Price,
                }
            };
        order.Subtotal = orderItems.Sum(oi => oi.Subtotal);
        order.Total = orderItems.Sum(oi => oi.Total);
        order.OrderItems = orderItems;
        context.SaveChanges();
        Log.Information("✅ Dynamic seed: demo orders added.");

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
        Log.Information("✅ Dynamic seed: demo suppliers added.");
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
        Log.Information("✅ Dynamic seed: demo warehouses added.");
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
            WarehouseId = 1
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
        Log.Information("✅ Dynamic seed: demo containers added.");
    }

    private static async Task SeedContainerStatusHistoryAsync(DatabaseContext context)
    {
        var active = await context.ContainerStatuses.FirstOrDefaultAsync(s => s.Description == "Active");
        var admin = await context.Users.FirstOrDefaultAsync(u => u.Email == "admin@market.local");

        if (active == null || admin == null)
        {
            return;
        }

        var containersWithoutStatus = await context.Containers
            .Where(c => !context.ContainerStatusHistories.Any(h => h.ContainerId == c.Id))
            .ToListAsync();

        if (containersWithoutStatus.Count == 0)
        {
            return;
        }

        foreach (var container in containersWithoutStatus)
        {
            context.ContainerStatusHistories.Add(new ContainerStatusHistoryEntity
            {
                ContainerId = container.Id,
                StatusId = active.Id,
                UserId = admin.Id,
                Date = DateTime.Now,
            });
        }

        await context.SaveChangesAsync();
        Log.Information("✅ Dynamic seed: {Count} container status history entries added.", containersWithoutStatus.Count);
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
            ContainerId = 2,
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
        Log.Information("✅ Dynamic seed: demo items added.");
    }

    private static async Task SeedTagsAsync(DatabaseContext context)
    {
        if (await context.Tags.AnyAsync())
        {
            return;
        }

        var tags = new List<TagEntity>
        {
            new() { Name = "Fragile" },
            new() { Name = "Perishable" },
            new() { Name = "Hazardous" },
            new() { Name = "Popular" },
            new() { Name = "Bestseller" },
        };

        context.Tags.AddRange(tags);
        await context.SaveChangesAsync();
        Log.Information("✅ Dynamic seed: demo tags added.");
    }

    private static async Task SeedItemTagsAsync(DatabaseContext context)
    {
        if (await context.ItemTags.AnyAsync())
        {
            return;
        }

        var waterBottle = await context.Item.FirstOrDefaultAsync(i => i.Name == "Natural water");
        var cocktaBottle = await context.Item.FirstOrDefaultAsync(i => i.Name == "Cockta soda");
        var perishable = await context.Tags.FirstOrDefaultAsync(t => t.Name == "Perishable");
        var bestseller = await context.Tags.FirstOrDefaultAsync(t => t.Name == "Bestseller");
        var popular = await context.Tags.FirstOrDefaultAsync(t => t.Name == "Popular");

        if (waterBottle == null || cocktaBottle == null || perishable == null || bestseller == null || popular == null)
        {
            return;
        }

        context.ItemTags.AddRange(
            new Item_TagEntity { ItemId = waterBottle.Id, TagId = perishable.Id },
            new Item_TagEntity { ItemId = waterBottle.Id, TagId = bestseller.Id },
            new Item_TagEntity { ItemId = cocktaBottle.Id, TagId = perishable.Id },
            new Item_TagEntity { ItemId = cocktaBottle.Id, TagId = popular.Id }
        );

        await context.SaveChangesAsync();
        Log.Information("✅ Dynamic seed: demo item tags added.");
    }


    /// <summary>
    /// Creates demo users if they don't already exist in the database.
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
            RoleId = Role.Admin,
            IsEnabled = true,
        };

        var manager = new UserEntity
        {
            Email = "manager@market.local",
            PasswordHash = hasher.HashPassword(null!, "Manager123!"),
            RoleId = Role.Manager,
            IsEnabled = true,
        };

        var user = new UserEntity
        {
            Email = "user@market.local",
            PasswordHash = hasher.HashPassword(null!, "User123!"),
            RoleId = Role.User,
            IsEnabled = true,
        };

        var dummyForSwagger = new UserEntity
        {
            Email = "string",
            PasswordHash = hasher.HashPassword(null!, "string"),
            RoleId = Role.User,
            IsEnabled = true,
        };
        var dummyForTests = new UserEntity
        {
            Email = "test",
            PasswordHash = hasher.HashPassword(null!, "test123"),
            RoleId = Role.User,
            IsEnabled = true,
        };
        context.Users.AddRange(admin, manager, user, dummyForSwagger, dummyForTests);
        await context.SaveChangesAsync();

        Log.Information("✅ Dynamic seed: demo users added.");
    }

    private static async Task SeedStorageIdentityAsync(DatabaseContext context)
    {
        var warehouse = await context.Warehouses.FirstOrDefaultAsync();

        if (warehouse == null)
        {
            warehouse = new WarehouseEntity { Name = "Main Warehouse" };
            context.Warehouses.Add(warehouse);
            await context.SaveChangesAsync();
            Log.Information("✅ Dynamic seed: demo warehouse added.");
        }

        var privilegeCodes = new[]
        {
            Priviledges.WarehouseRead,
            Priviledges.WarehouseUpdate,
            Priviledges.ContainerRead,
            Priviledges.ContainerCreate,
            Priviledges.ContainerUpdate,
            Priviledges.ContainerDelete,
            Priviledges.ItemRead,
            Priviledges.ItemCreate,
            Priviledges.ItemUpdate,
            Priviledges.ItemDelete,
        };

        var existingPrivileges = await context.Priviledges
            .Where(privilege => privilegeCodes.Contains(privilege.Code))
            .ToDictionaryAsync(privilege => privilege.Code, privilege => privilege.Id);

        var nextPrivilegeId = await context.Priviledges.AnyAsync()
            ? await context.Priviledges.MaxAsync(privilege => privilege.Id) + 1
            : 1;

        foreach (var code in privilegeCodes.Where(code => !existingPrivileges.ContainsKey(code)))
        {
            context.Priviledges.Add(new PriviledgeEntity
            {
                Id = nextPrivilegeId++,
                Code = code,
                Description = code,
            });
        }

        await context.SaveChangesAsync();

        var privilegeMap = await context.Priviledges
            .ToDictionaryAsync(privilege => privilege.Code, privilege => privilege.Id);

        var ownerGroup = await context.PriviledgeGroups
            .FirstOrDefaultAsync(group => group.WarehouseId == warehouse.Id && group.Name == "Owner");

        if (ownerGroup == null)
        {
            ownerGroup = new PriviledgeGroupEntity
            {
                Name = "Owner",
                WarehouseId = warehouse.Id,
            };

            context.PriviledgeGroups.Add(ownerGroup);
            await context.SaveChangesAsync();
        }

        var readerGroup = await context.PriviledgeGroups
            .FirstOrDefaultAsync(group => group.WarehouseId == warehouse.Id && group.Name == "Reader");

        if (readerGroup == null)
        {
            readerGroup = new PriviledgeGroupEntity
            {
                Name = "Reader",
                WarehouseId = warehouse.Id,
            };

            context.PriviledgeGroups.Add(readerGroup);
            await context.SaveChangesAsync();
        }

        foreach (var privilegeCode in privilegeCodes)
        {
            var privilegeId = privilegeMap[privilegeCode];

            var ownerLinkExists = await context.PriviledgeGroupsPriviledges.AnyAsync(link =>
                link.PriviledgeGroupId == ownerGroup.Id && link.PriviledgeId == privilegeId);

            if (!ownerLinkExists)
            {
                context.PriviledgeGroupsPriviledges.Add(new PriviledgeGroup_PriviledgeEntity
                {
                    PriviledgeGroupId = ownerGroup.Id,
                    PriviledgeId = privilegeId,
                });
            }
        }

        var readerPrivilegeCodes = new[] { Priviledges.WarehouseRead, Priviledges.ContainerRead, Priviledges.ItemRead };
        foreach (var readerPrivilegeCode in readerPrivilegeCodes)
        {
            var readerPrivilegeId = privilegeMap[readerPrivilegeCode];
            var readerLinkExists = await context.PriviledgeGroupsPriviledges.AnyAsync(link =>
                link.PriviledgeGroupId == readerGroup.Id && link.PriviledgeId == readerPrivilegeId);

            if (!readerLinkExists)
            {
                context.PriviledgeGroupsPriviledges.Add(new PriviledgeGroup_PriviledgeEntity
                {
                    PriviledgeGroupId = readerGroup.Id,
                    PriviledgeId = readerPrivilegeId,
                });
            }
        }

        await context.SaveChangesAsync();

        // Admin and Manager both get full (Owner) warehouse access — nothing in the business
        // rules restricts Manager within a warehouse. User demonstrates the Reader restriction.
        async Task EnsureAssignment(string email, PriviledgeGroupEntity group)
        {
            var user = await context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return;

            var assignmentExists = await context.WarehouseUsers.AnyAsync(link =>
                link.WarehouseId == warehouse.Id &&
                link.UserId == user.Id &&
                link.PriviledgeGroupId == group.Id);

            if (!assignmentExists)
            {
                context.WarehouseUsers.Add(new Warehouse_UserEntity
                {
                    WarehouseId = warehouse.Id,
                    UserId = user.Id,
                    PriviledgeGroupId = group.Id,
                });
            }
        }

        await EnsureAssignment("admin@market.local", ownerGroup);
        await EnsureAssignment("manager@market.local", ownerGroup);
        await EnsureAssignment("user@market.local", readerGroup);

        await context.SaveChangesAsync();
        Log.Information("✅ Dynamic seed: warehouse identity groups and assignments added.");
    }
}