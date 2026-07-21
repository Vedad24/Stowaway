using Market.Domain.Entities.Sales;
using Stowaway.Domain.Entities.Identity;
using Stowaway.Domain.Entities.Sales;
using Stowaway.Domain.Entities.Storage;
using Stowaway.Domain.Entities.Storage.StorageIdentity;

namespace Market.Application.Abstractions;

// Application layer
public interface IAppDbContext
{
    public DbSet<ContainerTypeEntity> ContainerTypes { get;}
    //DbSet<ContainerEntity> Containers { get; }
    DbSet<WarehouseEntity> Warehouses { get; }
    DbSet<ContainerEntity> Containers { get; }
    DbSet<ItemEntity> Item { get; }
    DbSet<SupplierEntity> Suppliers { get; }
    //StorageIdentity
    DbSet<PriviledgeEntity> Priviledges { get; }
    DbSet<PriviledgeGroup_PriviledgeEntity> PriviledgeGroupsPriviledges { get; }
    DbSet<PriviledgeGroupEntity> PriviledgeGroups { get; }
    DbSet<Warehouse_UserEntity> WarehouseUsers { get; }
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
    public DbSet<CartItemEntity> CartItems { get; }
    Task<int> SaveChangesAsync(CancellationToken ct);
}