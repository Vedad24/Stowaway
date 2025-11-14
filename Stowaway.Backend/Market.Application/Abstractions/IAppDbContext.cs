namespace Market.Application.Abstractions;

// Application layer
public interface IAppDbContext
{
    //DbSet<ContainerEntity> Containers { get; }
    //DbSet<WarehouseEntity> Warehouses { get; }
    DbSet<UserEntity> Users { get; }
    DbSet<RefreshTokenEntity> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);
}