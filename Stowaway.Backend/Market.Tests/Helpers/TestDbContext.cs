using Market.Application.Abstractions;
using Market.Domain.Entities.Identity;
using Market.Domain.Entities.Sales;
using Market.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Stowaway.Domain.Entities.Identity;
using Stowaway.Domain.Entities.Sales;
using Stowaway.Domain.Entities.Storage;
using Stowaway.Domain.Entities.Storage.StorageIdentity;

namespace Market.Tests.Helpers;

public sealed class TestDbContext : DbContext, IAppDbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<ContainerTypeEntity> ContainerTypes => Set<ContainerTypeEntity>();
    public DbSet<WarehouseEntity> Warehouses => Set<WarehouseEntity>();
    public DbSet<ContainerEntity> Containers => Set<ContainerEntity>();
    public DbSet<ContainerStatusEntity> ContainerStatuses => Set<ContainerStatusEntity>();
    public DbSet<ContainerStatusHistoryEntity> ContainerStatusHistories => Set<ContainerStatusHistoryEntity>();
    public DbSet<ItemEntity> Item => Set<ItemEntity>();
    public DbSet<Item_TagEntity> ItemTags => Set<Item_TagEntity>();
    public DbSet<TagEntity> Tags => Set<TagEntity>();
    public DbSet<SupplierEntity> Suppliers => Set<SupplierEntity>();

    public DbSet<PriviledgeEntity> Priviledges => Set<PriviledgeEntity>();
    public DbSet<PriviledgeGroup_PriviledgeEntity> PriviledgeGroupsPriviledges => Set<PriviledgeGroup_PriviledgeEntity>();
    public DbSet<PriviledgeGroupEntity> PriviledgeGroups => Set<PriviledgeGroupEntity>();
    public DbSet<Warehouse_UserEntity> WarehouseUsers => Set<Warehouse_UserEntity>();

    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<RoleEntity> Roles => Set<RoleEntity>();
    public DbSet<PermissionEntity> Permissions => Set<PermissionEntity>();
    public DbSet<Permission_RoleEntity> PermissionRoles => Set<Permission_RoleEntity>();
    public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();

    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
    public DbSet<OrderItemEntity> OrderItems => Set<OrderItemEntity>();
    public DbSet<OrderStatusEntity> OrderStatuses => Set<OrderStatusEntity>();
    public DbSet<CartItemEntity> CartItems => Set<CartItemEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
    }

    public static TestDbContext Create()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestDbContext(options);
    }
}
