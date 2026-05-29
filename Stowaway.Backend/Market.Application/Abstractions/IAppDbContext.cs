using Stowaway.Domain.Entities.Identity;
using Stowaway.Domain.Entities.Sales;
using Stowaway.Domain.Entities.Storage;

namespace Market.Application.Abstractions;

// Application layer
public interface IAppDbContext
{
    public DbSet<ContainerTypeEntity> ContainerTypes { get;}
    //DbSet<ContainerEntity> Containers { get; }
    DbSet<WarehouseEntity> Warehouses { get; }

    DbSet<ItemEntity> Item { get; }
    DbSet<SupplierEntity> Suppliers { get; }

    //Identity
    DbSet<UserEntity> Users { get; }
    DbSet<RoleEntity> Roles { get; }
    DbSet<PermissionEntity> Permissions { get; }
    DbSet<Permission_RoleEntity> PermissionRoles { get; }
    DbSet<RefreshTokenEntity> RefreshTokens { get; }

    //Sales
    public DbSet<OrderEntity> Orders { get; }
    public DbSet<OrderItemEntity> OrderItems { get; }
    public DbSet<OrderStatusEntity> OrderStatuses { get; }
    Task<int> SaveChangesAsync(CancellationToken ct);
}