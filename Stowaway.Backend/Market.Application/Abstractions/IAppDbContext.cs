namespace Market.Application.Abstractions;

// Application layer
public interface IAppDbContext
{
    DbSet<ContainerEntity> Products { get; }
    DbSet<WarehouseEntity> ProductCategories { get; }
    DbSet<MarketUserEntity> Users { get; }
    DbSet<RefreshTokenEntity> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);
}