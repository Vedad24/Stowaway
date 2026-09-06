using Stowaway.Application.Abstractions;
using Stowaway.Domain.Entities.Sales;
using Stowaway.Domain.Entities.Identity;
using Stowaway.Domain.Entities.Sales;
using Stowaway.Domain.Entities.Storage;
using Stowaway.Domain.Entities.Storage.StorageIdentity;

namespace Stowaway.Infrastructure.Database;

public partial class DatabaseContext : DbContext, IAppDbContext
{
    //Identity
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<RoleEntity> Roles => Set<RoleEntity>();
    public DbSet<PermissionEntity> Permissions => Set<PermissionEntity>();
    public DbSet<Permission_RoleEntity> PermissionRoles => Set<Permission_RoleEntity>();
    public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();

    //Sales
    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
    public DbSet<OrderItemEntity> OrderItems => Set<OrderItemEntity>();
    public DbSet<OrderStatusEntity> OrderStatuses => Set<OrderStatusEntity>();
    public DbSet<CartItemEntity> CartItems => Set<CartItemEntity>();
    public DbSet<ProcessedStripeEventEntity> ProcessedStripeEvents => Set<ProcessedStripeEventEntity>();

    //Storage
    public DbSet<ContainerEntity> Containers => Set<ContainerEntity>();
    public DbSet<ContainerTypeEntity> ContainerTypes => Set<ContainerTypeEntity>();
    public DbSet<Item_TagEntity> ItemTags => Set<Item_TagEntity>();
    public DbSet<Item_UserFavouriteEntity> ItemFavourites => Set<Item_UserFavouriteEntity>();
    public DbSet<ItemImageEntity> ItemImages => Set<ItemImageEntity>();
    public DbSet<ItemEntity> Item => Set<ItemEntity>();
    public DbSet<SupplierEntity> Suppliers => Set<SupplierEntity>();
    public DbSet<TagEntity> Tags => Set<TagEntity>();
    public DbSet<WarehouseEntity> Warehouses => Set<WarehouseEntity>();
    public DbSet<ContainerStatusHistoryEntity> ContainerStatusHistories => Set<ContainerStatusHistoryEntity>();
    public DbSet<ContainerStatusEntity> ContainerStatuses => Set<ContainerStatusEntity>();

    //StorageIdentity
    public DbSet<Warehouse_UserEntity> WarehouseUsers => Set<Warehouse_UserEntity>();
    public DbSet<PriviledgeEntity> Priviledges => Set<PriviledgeEntity>();
    public DbSet<PriviledgeGroupEntity> PriviledgeGroups => Set<PriviledgeGroupEntity>();
    public DbSet<PriviledgeGroup_PriviledgeEntity> PriviledgeGroupsPriviledges => Set<PriviledgeGroup_PriviledgeEntity>();

    private readonly TimeProvider _clock;
    public DatabaseContext(DbContextOptions<DatabaseContext> options, TimeProvider clock) : base(options)
    {
        _clock = clock;
    }
}