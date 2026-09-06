using Stowaway.Domain.Entities.Sales;

namespace Stowaway.Infrastructure.Database.Configurations.Sales;

public sealed class ProcessedStripeEventEntityConfiguration : IEntityTypeConfiguration<ProcessedStripeEventEntity>
{
    public void Configure(EntityTypeBuilder<ProcessedStripeEventEntity> b)
    {
        b.HasKey(x => x.EventId);

        b.Property(x => x.EventId)
            .HasMaxLength(255);

        b.Property(x => x.ProcessedAtUtc)
            .IsRequired();
    }
}
