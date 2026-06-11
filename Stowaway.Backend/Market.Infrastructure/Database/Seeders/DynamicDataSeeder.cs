using Market.Shared.Constants;
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
        await SeedPermissionsAsync(context);
        await SeedPrivilegesAsync(context);
        await SeedRolesAsync(context);
        await SeedUsersAsync(context);
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

        var existingPrivileges = await context.Permissions
            .Where(permission => privilegeNames.Contains(permission.Description))
            .Select(permission => permission.Description)
            .ToListAsync();

        var missingPrivileges = privilegeNames
            .Where(name => !existingPrivileges.Contains(name))
            .Select((name, index) => new PermissionEntity
            {
                Description = name
            })
            .ToList();

        if (missingPrivileges.Count == 0)
        {
            Console.WriteLine("✅ Dynamic seed: warehouse privileges already exist.");
            return;
        }

        var nextId = await context.Permissions.AnyAsync()
            ? await context.Permissions.MaxAsync(permission => permission.Id) + 1
            : 1;

        foreach (var privilege in missingPrivileges)
        {
            privilege.Id = nextId++;
            context.Permissions.Add(privilege);
        }

        await context.SaveChangesAsync();
        Console.WriteLine($"✅ Dynamic seed: {missingPrivileges.Count} warehouse privileges added.");
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