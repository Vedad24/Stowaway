using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stowaway.Domain.Entities.Storage.StorageIdentity;

namespace Stowaway.Infrastructure.Database.Configurations.Storage.StorageIdentity
{
    public class Warehouse_UserEntityConfiguration : IEntityTypeConfiguration<Warehouse_UserEntity>
    {
        public void Configure(EntityTypeBuilder<Warehouse_UserEntity> builder)
        {
            builder.HasKey(wu => new { wu.WarehouseId, wu.UserId, wu.PriviledgeGroupId });

            builder.HasOne(wu => wu.Warehouse)
                .WithMany()
                .HasForeignKey(wu => wu.WarehouseId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(wu => wu.User)
                .WithMany()
                .HasForeignKey(wu => wu.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(wu => wu.PriviledgeGroup)
                .WithMany()
                .HasForeignKey(wu => wu.PriviledgeGroupId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}