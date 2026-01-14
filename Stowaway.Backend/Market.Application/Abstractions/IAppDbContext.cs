using Stowaway.Domain.Entities.Identity;
using Stowaway.Domain.Entities.Storage;

namespace Market.Application.Abstractions;

// Application layer
public interface IAppDbContext
{
    //DbSet<ContainerEntity> Containers { get; }
    DbSet<WarehouseEntity> Warehouses { get; }

    DbSet<ItemEntity> Item { get; }
    DbSet<SupplierEntity> Suppliers { get; }

    DbSet<UserEntity> Users { get; }
    DbSet<RoleEntity> Roles { get; }
    DbSet<RefreshTokenEntity> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);
}