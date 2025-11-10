namespace Market.Infrastructure.Database.Configurations.Catelog;

public class ProductCategoryConfiguration : IEntityTypeConfiguration<WarehouseEntity>
{
    public void Configure(EntityTypeBuilder<WarehouseEntity> builder)
    {
        builder
            .ToTable("ProductCategories");

        builder
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(WarehouseEntity.Constraints.NameMaxLength);

        builder
            .Property(x => x.IsEnabled)
            .IsRequired();

    }
}
