using Market.Shared.Constants;
using Stowaway.Domain.Entities.Identity;
using Stowaway.Domain.Entities.Sales;
using Stowaway.Domain.Entities.Storage;
using Stowaway.Domain.Entities.Storage.StorageIdentity;
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
        return;
        //await SeedProductCategoriesAsync(context);
        await SeedContainerTypesAsync(context);
        await SeedOrderStatusAsync(context);
        await SeedPermissionsAsync(context);
        await SeedPrivilegesAsync(context);
        await SeedRolesAsync(context);
        await SeedUsersAsync(context);
        await SeedSupplierAsync(context);
        await SeedWarehouseAsync(context);
        await SeedContainersAsync(context);
        await SeedItemsAsync(context);
        await SeedOrdersAsync(context);
        await SeedStorageIdentityAsync(context);
        await SeedRolePermissionsAsync(context);
    }
    private static async Task SeedPermissionsAsync(DatabaseContext context)
    {
        if (await context.Permissions.AnyAsync())
            return;

        var permissions = new List<PermissionEntity>
        {
            new() { Id = 1, Description = Permissions.UsersRead },
            new() { Id = 2, Description = Permissions.UsersCreate },
            new() { Id = 3, Description = Permissions.UsersUpdate },
            new() { Id = 4, Description = Permissions.UsersDelete },
            new() { Id = 5, Description = Permissions.RolesRead },
            new() { Id = 6, Description = Permissions.RolesCreate },
            new() { Id = 7, Description = Permissions.RolesUpdate },
            new() { Id = 8, Description = Permissions.RolesDelete }
        };

        context.Permissions.AddRange(permissions);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Dynamic seed: permissions added.");
    }

    private static async Task SeedPrivilegesAsync(DatabaseContext context)
    {
        var privilegeNames = new[]
        {
            Priviledges.WarehouseRead,
            Priviledges.WarehouseCreate,
            Priviledges.WarehouseUpdate,
            Priviledges.WarehouseDelete,
            Priviledges.InventoryRead,
            Priviledges.InventoryCreate,
            Priviledges.InventoryUpdate,
            Priviledges.InventoryDelete,
            Priviledges.ProductRead,
            Priviledges.ProductCreate,
            Priviledges.ProductUpdate,
            Priviledges.ProductDelete,
            Priviledges.OrderRead,
            Priviledges.OrderCreate,
            Priviledges.OrderUpdate,
            Priviledges.OrderDelete,
            Priviledges.SupplierRead,
            Priviledges.SupplierCreate,
            Priviledges.SupplierUpdate,
            Priviledges.SupplierDelete,
            Priviledges.UsersManage,
            Priviledges.RolesManage,
            Priviledges.WarehouseUsersManage,
            Priviledges.ReportsRead,
            Priviledges.ReportsExport,
        };

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
            Console.WriteLine("✅ Dynamic seed: warehouse privileges already exist.");
            return;
        }

        context.Priviledges.AddRange(missingPrivileges);

        await context.SaveChangesAsync();
        Console.WriteLine($"✅ Dynamic seed: {missingPrivileges.Count} warehouse privileges added.");
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
            RoleId = Role.Admin,
            IsEnabled = true,
        };

        var user = new UserEntity
        {
            Email = "manager@market.local",
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
        context.Users.AddRange(admin, user, dummyForSwagger, dummyForTests);
        await context.SaveChangesAsync();

        Console.WriteLine("✅ Dynamic seed: demo users added.");
    }

    private static async Task SeedStorageIdentityAsync(DatabaseContext context)
    {
        var warehouse = await context.Warehouses.FirstOrDefaultAsync();

        if (warehouse == null)
        {
            warehouse = new WarehouseEntity { Name = "Main Warehouse" };
            context.Warehouses.Add(warehouse);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Dynamic seed: demo warehouse added.");
        }

        var privilegeCodes = new[]
        {
            Priviledges.WarehouseRead,
            Priviledges.WarehouseCreate,
            Priviledges.WarehouseUpdate,
            Priviledges.WarehouseDelete,
            Priviledges.InventoryRead,
            Priviledges.InventoryCreate,
            Priviledges.InventoryUpdate,
            Priviledges.InventoryDelete,
            Priviledges.ProductRead,
            Priviledges.ProductCreate,
            Priviledges.ProductUpdate,
            Priviledges.ProductDelete,
            Priviledges.OrderRead,
            Priviledges.OrderCreate,
            Priviledges.OrderUpdate,
            Priviledges.OrderDelete,
            Priviledges.SupplierRead,
            Priviledges.SupplierCreate,
            Priviledges.SupplierUpdate,
            Priviledges.SupplierDelete,
            Priviledges.UsersManage,
            Priviledges.RolesManage,
            Priviledges.WarehouseUsersManage,
            Priviledges.ReportsRead,
            Priviledges.ReportsExport,
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

        var readerPrivilegeId = privilegeMap[Priviledges.WarehouseRead];
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

        await context.SaveChangesAsync();

        var admin = await context.Users.SingleOrDefaultAsync(user => user.Email == "admin@market.local");
        if (admin != null)
        {
            var adminAssignmentExists = await context.WarehouseUsers.AnyAsync(link =>
                link.WarehouseId == warehouse.Id &&
                link.UserId == admin.Id &&
                link.PriviledgeGroupId == ownerGroup.Id);

            if (!adminAssignmentExists)
            {
                context.WarehouseUsers.Add(new Warehouse_UserEntity
                {
                    WarehouseId = warehouse.Id,
                    UserId = admin.Id,
                    PriviledgeGroupId = ownerGroup.Id,
                });
            }
        }

        var readerUser = await context.Users.SingleOrDefaultAsync(user => user.Email == "manager@market.local");
        if (readerUser != null)
        {
            var readerAssignmentExists = await context.WarehouseUsers.AnyAsync(link =>
                link.WarehouseId == warehouse.Id &&
                link.UserId == readerUser.Id &&
                link.PriviledgeGroupId == readerGroup.Id);

            if (!readerAssignmentExists)
            {
                context.WarehouseUsers.Add(new Warehouse_UserEntity
                {
                    WarehouseId = warehouse.Id,
                    UserId = readerUser.Id,
                    PriviledgeGroupId = readerGroup.Id,
                });
            }
        }

        await context.SaveChangesAsync();
        Console.WriteLine("✅ Dynamic seed: warehouse identity groups and assignments added.");
    }

    private static async Task SeedRolePermissionsAsync(DatabaseContext context)
    {
        if (!await context.Permissions.AnyAsync())
            return;

        var permissions = await context.Permissions
            .AsNoTracking()
            .ToListAsync();

        var existingAssignments = await context.PermissionRoles
            .Select(permissionRole => new { permissionRole.RoleId, permissionRole.PermissionId })
            .ToListAsync();

        var existingSet = new HashSet<(Role RoleId, int PermissionId)>(
            existingAssignments.Select(item => (item.RoleId, item.PermissionId)));

        var permissionRoleAssignments = new List<Permission_RoleEntity>();

        foreach (var permission in permissions)
        {
            if (!existingSet.Contains((Role.Admin, permission.Id)))
            {
                permissionRoleAssignments.Add(new Permission_RoleEntity
                {
                    RoleId = Role.Admin,
                    PermissionId = permission.Id,
                });
            }

            if (permission.Description.EndsWith(".Read", StringComparison.OrdinalIgnoreCase) &&
                !existingSet.Contains((Role.User, permission.Id)))
            {
                permissionRoleAssignments.Add(new Permission_RoleEntity
                {
                    RoleId = Role.User,
                    PermissionId = permission.Id,
                });
            }
        }

        if (permissionRoleAssignments.Count == 0)
        {
            Console.WriteLine("✅ Dynamic seed: role permissions already exist.");
            return;
        }

        context.PermissionRoles.AddRange(permissionRoleAssignments);
        await context.SaveChangesAsync();

        Console.WriteLine($"✅ Dynamic seed: {permissionRoleAssignments.Count} role permissions added.");
    }
}