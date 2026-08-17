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
        await SeedContainerStatusesAsync(context);
        await SeedContainerStatusHistoryAsync(context);
        await SeedItemsAsync(context);
        await SeedTagsAsync(context);
        await SeedItemTagsAsync(context);
        await SeedOrdersAsync(context);
        await SeedStorageIdentityAsync(context);
        await SeedRolePermissionsAsync(context);
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
            Permissions.WarehouseUsersManage,
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
            Console.WriteLine("✅ Dynamic seed: permissions already exist.");
            return;
        }

        context.Permissions.AddRange(missingPermissions);
        await context.SaveChangesAsync();
        Console.WriteLine($"✅ Dynamic seed: {missingPermissions.Count} permissions added.");
    }

    private static async Task SeedPrivilegesAsync(DatabaseContext context)
    {
        var privilegeNames = new[]
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
            Console.WriteLine("✅ Dynamic seed: demo container types added.");
        }
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
        Console.WriteLine("✅ Dynamic seed: demo container statuses added.");
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
        Console.WriteLine($"✅ Dynamic seed: {containersWithoutStatus.Count} container status history entries added.");
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
        Console.WriteLine("✅ Dynamic seed: demo tags added.");
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
        Console.WriteLine("✅ Dynamic seed: demo item tags added.");
    }


    /// <summary>
    /// Kreira demo korisnike ako ih još nema u bazi.
    /// </summary>

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
        Console.WriteLine("✅ Dynamic seed: roles already exist.");
        return;
    }

    context.Roles.AddRange(missingRoles);
    await context.SaveChangesAsync();

    Console.WriteLine($"✅ Dynamic seed: {missingRoles.Count} roles added.");
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
        Console.WriteLine("✅ Dynamic seed: warehouse identity groups and assignments added.");
    }

    // Explicit per-role permission matrix. Admin gets every permission that exists.
    // Manager gets everything except Roles.Create/Update/Delete.
    // User only gets its own module access (Self, Warehouse.Read, Container/Item/Tag/Supplier.Read, Cart).
    private static readonly string[] ManagerOnlyPermissions =
    {
        Permissions.UsersRead, Permissions.UsersCreate, Permissions.UsersUpdate, Permissions.UsersDelete,
        Permissions.RolesRead,
        Permissions.WarehouseCreate, Permissions.WarehouseUpdate, Permissions.WarehouseDelete,
        Permissions.OrderRead, Permissions.OrderCreate,
        Permissions.SupplierCreate, Permissions.SupplierUpdate, Permissions.SupplierDelete,
        Permissions.WarehouseUsersManage,
    };

    private static readonly string[] AdminOnlyPermissions =
    {
        Permissions.RolesCreate, Permissions.RolesUpdate, Permissions.RolesDelete,
        Permissions.OrderUpdate, Permissions.OrderDelete,
    };

    private static readonly string[] SharedByAllRolesPermissions =
    {
        Permissions.UsersSelfRead, Permissions.UsersSelfUpdate, Permissions.UsersSelfDelete,
        Permissions.WarehouseRead,
        Permissions.ContainerRead, Permissions.ContainerCreate, Permissions.ContainerUpdate, Permissions.ContainerDelete,
        Permissions.ItemRead, Permissions.ItemCreate, Permissions.ItemUpdate, Permissions.ItemDelete,
        Permissions.TagRead, Permissions.TagCreate,
        Permissions.SupplierRead,
        Permissions.CartManage,
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

        // Reconcile: drop any previously-granted row (e.g. from the old heuristic seeder)
        // that the new explicit matrix no longer grants.
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
            Console.WriteLine("✅ Dynamic seed: role permissions already up to date.");
            return;
        }

        await context.SaveChangesAsync();

        Console.WriteLine($"✅ Dynamic seed: {toAdd.Count} role permissions added, {toRemove.Count} stale role permissions removed.");
    }
}