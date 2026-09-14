using Stowaway.Shared.Constants;
using Serilog;
using Stowaway.Domain.Entities.Identity;
using Stowaway.Domain.Entities.Sales;
using Stowaway.Domain.Entities.Storage;
using Stowaway.Domain.Entities.Storage.StorageIdentity;
using System.Numerics;

namespace Stowaway.Infrastructure.Database.Seeders;

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
            OrderDate = DateTime.UtcNow,
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

        var suppliers = new List<SupplierEntity>
        {
            new() { Name = "AquaTerm", Description = "Beverage company", Address = "King Tomfame Street, Mostar", TotalDeliveries = 50, FailedDeliveries = 3 },
            new() { Name = "NovaCircuit", Description = "Consumer electronics distributor", Address = "Technology Park 12, Sarajevo", TotalDeliveries = 30, FailedDeliveries = 1 },
            new() { Name = "PackRight", Description = "Packaging materials supplier", Address = "Industrial bb, Tuzla", TotalDeliveries = 75, FailedDeliveries = 5 },
            new() { Name = "GreenField Produce", Description = "Fresh produce supplier", Address = "Agriculture 4, Banja Luka", TotalDeliveries = 60, FailedDeliveries = 8 },
            new() { Name = "IronWorks Hardware", Description = "Hardware and fasteners supplier", Address = "Handywork 9, Zenica", TotalDeliveries = 40, FailedDeliveries = 2 },
            new() { Name = "Woven Threads", Description = "Textile goods company", Address = "Textile 21, Mostar", TotalDeliveries = 20, FailedDeliveries = 0 },
        };

        context.Suppliers.AddRange(suppliers);
        await context.SaveChangesAsync();
        Log.Information("✅ Dynamic seed: demo suppliers added.");
    }

    private static async Task SeedWarehouseAsync(DatabaseContext context)
    {
        if (await context.Warehouses.AnyAsync())
        {
            return;
        }

        var warehouses = new List<WarehouseEntity>
        {
            new() { Name = "North Distribution Center", Description = "Main hub for inbound stock.", City = "Sarajevo", Address = "Industry zone bb", Capacity = 60, isEnabled = true },
            new() { Name = "South Depot", Description = "Regional depot for southern deliveries.", City = "Mostar", Address = "Brothers Fejic 30", Capacity = 45, isEnabled = true },
            new() { Name = "East Storage Facility", Description = "Overflow and bulk storage.", City = "Tuzla", Address = "Warehouse 5", Capacity = 30, isEnabled = true },
            new() { Name = "West Cold Store", Description = "Refrigerated storage for perishables.", City = "Banja Luka", Address = "Coldroom 2", Capacity = 20, isEnabled = true },
            new() { Name = "Old Riverside Warehouse", Description = "Legacy site, being phased out.", City = "Mostar", Address = "River 8", Capacity = 15, isEnabled = false },
        };

        context.Warehouses.AddRange(warehouses);
        await context.SaveChangesAsync();
        Log.Information("✅ Dynamic seed: demo warehouses added.");
    }

    private static async Task SeedContainersAsync(DatabaseContext context)
    {
        if (await context.Containers.AnyAsync())
        {
            return;
        }

        var palletRackA1 = new ContainerEntity { Name = "Pallet Rack A1", ContainerTypeId = 4, ParentContainerId = null, WarehouseId = 1 };
        var rackC1 = new ContainerEntity { Name = "Rack C1", ContainerTypeId = 4, ParentContainerId = null, WarehouseId = 3 };

        context.Containers.AddRange(palletRackA1, rackC1);
        await context.SaveChangesAsync();

        var containers = new List<ContainerEntity>
        {
            new() { Name = "Bin A1-01", ContainerTypeId = 1, ParentContainerId = palletRackA1.Id, WarehouseId = 1 },
            new() { Name = "Shelf A2", ContainerTypeId = 3, ParentContainerId = null, WarehouseId = 1 },
            new() { Name = "Crate B1", ContainerTypeId = 2, ParentContainerId = null, WarehouseId = 2 },
            new() { Name = "Crate B2", ContainerTypeId = 2, ParentContainerId = null, WarehouseId = 2 },
            new() { Name = "Tote C1-1", ContainerTypeId = 1, ParentContainerId = rackC1.Id, WarehouseId = 3 },
            new() { Name = "Cold Bin D1", ContainerTypeId = 2, ParentContainerId = null, WarehouseId = 4 },
            new() { Name = "Cold Bin D2", ContainerTypeId = 2, ParentContainerId = null, WarehouseId = 4 },
            new() { Name = "Old Shelf E1", ContainerTypeId = 3, ParentContainerId = null, WarehouseId = 5 },
            new() { Name = "Old Crate E2", ContainerTypeId = 1, ParentContainerId = null, WarehouseId = 5 },
        };

        context.Containers.AddRange(containers);
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

        var containers = await context.Containers.ToDictionaryAsync(c => c.Name, c => c.Id);
        var suppliers = await context.Suppliers.ToDictionaryAsync(s => s.Name, s => s.Id);

        // Quantity per container is kept within that container's ContainerType.MaxItems
        // (Tiny=10, Small=50, Medium=100, Large=500 — see StaticDataSeeder.SeedContainerTypesAsync).
        var items = new List<ItemEntity>
        {
            // Bin A1-01 (Tiny, max 10): 4 + 6 = 10
            new() { Name = "Natural Spring Water 0.5L", Description = "0.5l plastic bottle", Quantity = 4, ContainerId = containers["Bin A1-01"], SupplierId = suppliers["AquaTerm"] },
            new() { Name = "Citrus Soda Can 0.33L", Description = "0.33l aluminium can", Quantity = 6, ContainerId = containers["Bin A1-01"], SupplierId = suppliers["AquaTerm"] },
            // Crate B1 (Small, max 50)
            new() { Name = "Iced Tea Bottle 1L", Description = "1l glass bottle", Quantity = 30, ContainerId = containers["Crate B1"], SupplierId = suppliers["AquaTerm"] },
            // Shelf A2 (Medium, max 100): 20 + 45 + 30 = 95
            new() { Name = "Wireless Mouse", Description = "2.4GHz wireless mouse", Quantity = 20, ContainerId = containers["Shelf A2"], SupplierId = suppliers["NovaCircuit"] },
            new() { Name = "USB-C Charging Cable", Description = "1m braided cable", Quantity = 45, ContainerId = containers["Shelf A2"], SupplierId = suppliers["NovaCircuit"] },
            // Crate B2 (Small, max 50)
            new() { Name = "Bluetooth Speaker", Description = "Portable waterproof speaker", Quantity = 15, ContainerId = containers["Crate B2"], SupplierId = suppliers["NovaCircuit"] },
            // Tote C1-1 (Tiny, max 10): 6 + 4 = 10
            new() { Name = "Corrugated Box Small", Description = "30x20x20cm shipping box", Quantity = 6, ContainerId = containers["Tote C1-1"], SupplierId = suppliers["PackRight"] },
            new() { Name = "Bubble Wrap Roll", Description = "50m protective wrap", Quantity = 4, ContainerId = containers["Tote C1-1"], SupplierId = suppliers["PackRight"] },
            // Pallet Rack A1 (Large, max 500)
            new() { Name = "Packing Tape Set", Description = "6-pack clear tape", Quantity = 90, ContainerId = containers["Pallet Rack A1"], SupplierId = suppliers["PackRight"] },
            // Cold Bin D1 (Small, max 50): 25 + 20 = 45
            new() { Name = "Fresh Apples Crate", Description = "10kg produce crate", Quantity = 25, ContainerId = containers["Cold Bin D1"], SupplierId = suppliers["GreenField Produce"] },
            new() { Name = "Frozen Berries Pack", Description = "1kg frozen pack", Quantity = 20, ContainerId = containers["Cold Bin D1"], SupplierId = suppliers["GreenField Produce"] },
            // Cold Bin D2 (Small, max 50)
            new() { Name = "Organic Carrots Bag", Description = "5kg produce bag", Quantity = 30, ContainerId = containers["Cold Bin D2"], SupplierId = suppliers["GreenField Produce"] },
            // Rack C1 (Large, max 500): 300 + 25 = 325
            new() { Name = "Steel Hex Bolts Box", Description = "M8 hex bolts, box of 500", Quantity = 300, ContainerId = containers["Rack C1"], SupplierId = suppliers["IronWorks Hardware"] },
            new() { Name = "Adjustable Wrench Set", Description = "3-piece wrench set", Quantity = 25, ContainerId = containers["Rack C1"], SupplierId = suppliers["IronWorks Hardware"] },
            // Old Shelf E1 (Medium, max 100): 80 + 15 = 95
            new() { Name = "Galvanized Nails Pack", Description = "2kg nail pack", Quantity = 80, ContainerId = containers["Old Shelf E1"], SupplierId = suppliers["IronWorks Hardware"] },
            new() { Name = "Wool Blanket", Description = "Single size wool blanket", Quantity = 15, ContainerId = containers["Old Shelf E1"], SupplierId = suppliers["Woven Threads"] },
            // Old Crate E2 (Tiny, max 10)
            new() { Name = "Cotton Bedsheet Set", Description = "Queen size cotton set", Quantity = 8, ContainerId = containers["Old Crate E2"], SupplierId = suppliers["Woven Threads"] },
            // Shelf A2 (Medium, max 100): running total with the two above = 95
            new() { Name = "Linen Tablecloth", Description = "150x250cm linen cloth", Quantity = 30, ContainerId = containers["Shelf A2"], SupplierId = suppliers["Woven Threads"] },
        };

        context.Item.AddRange(items);
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

        var waterBottle = await context.Item.FirstOrDefaultAsync(i => i.Name == "Natural Spring Water 0.5L");
        var berries = await context.Item.FirstOrDefaultAsync(i => i.Name == "Frozen Berries Pack");
        var perishable = await context.Tags.FirstOrDefaultAsync(t => t.Name == "Perishable");
        var bestseller = await context.Tags.FirstOrDefaultAsync(t => t.Name == "Bestseller");
        var popular = await context.Tags.FirstOrDefaultAsync(t => t.Name == "Popular");

        if (waterBottle == null || berries == null || perishable == null || bestseller == null || popular == null)
        {
            return;
        }

        context.ItemTags.AddRange(
            new Item_TagEntity { ItemId = waterBottle.Id, TagId = perishable.Id },
            new Item_TagEntity { ItemId = waterBottle.Id, TagId = bestseller.Id },
            new Item_TagEntity { ItemId = berries.Id, TagId = perishable.Id },
            new Item_TagEntity { ItemId = berries.Id, TagId = popular.Id }
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
            FirstName = "Admin",
            LastName = "AdminLastName",
            PasswordHash = hasher.HashPassword(null!, "Admin123!"),
            RoleId = Role.Admin,
            IsEnabled = true,
        };

        var manager = new UserEntity
        {
            Email = "manager@market.local",
            FirstName = "Manager",
            LastName = "ManagerLastName",
            PasswordHash = hasher.HashPassword(null!, "Manager123!"),
            RoleId = Role.Manager,
            IsEnabled = true,
        };

        var user = new UserEntity
        {
            Email = "user@market.local",
            FirstName = "User",
            LastName = "UserLastName",
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
        var warehouses = await context.Warehouses.ToListAsync();

        if (warehouses.Count == 0)
        {
            var mainWarehouse = new WarehouseEntity { Name = "Main Warehouse" };
            context.Warehouses.Add(mainWarehouse);
            await context.SaveChangesAsync();
            warehouses.Add(mainWarehouse);
            Log.Information("✅ Dynamic seed: demo warehouse added.");
        }

        var privilegeCodes = Priviledges.AllCodes;

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

        var readerPrivilegeCodes = new[] { Priviledges.WarehouseRead, Priviledges.ContainerRead, Priviledges.ItemRead };

        async Task EnsureAssignment(WarehouseEntity warehouse, string email, PriviledgeGroupEntity group)
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

        // Every warehouse gets its own Owner/Reader groups — Admin gets Owner access to
        // every warehouse. Manager/User only demonstrate the split on the first warehouse.
        for (var i = 0; i < warehouses.Count; i++)
        {
            var warehouse = warehouses[i];

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

            await EnsureAssignment(warehouse, "admin@market.local", ownerGroup);

            if (i == 0)
            {
                await EnsureAssignment(warehouse, "manager@market.local", ownerGroup);
                await EnsureAssignment(warehouse, "user@market.local", readerGroup);
            }
        }

        await context.SaveChangesAsync();
        Log.Information("✅ Dynamic seed: warehouse identity groups and assignments added.");
    }
}