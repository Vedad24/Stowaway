namespace Market.Infrastructure.Database.Configurations.Catelog;

public class ProductConfiguration : IEntityTypeConfiguration<ContainerEntity>
{
    public void Configure(EntityTypeBuilder<ContainerEntity> builder)
    {
        builder
            .ToTable("Products");

        builder
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(ContainerEntity.Constraints.NameMaxLength);

        builder
            .Property(x => x.Description)
            .HasMaxLength(ContainerEntity.Constraints.DescriptionMaxLength);

        builder
            .Property(x => x.Price)
            .HasPrecision(18, 2);

        builder
            .Property(x => x.StockQuantity)
            .IsRequired();

        builder
            .HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);// Restrict — do not allow deleting a category if it has products
    }
}