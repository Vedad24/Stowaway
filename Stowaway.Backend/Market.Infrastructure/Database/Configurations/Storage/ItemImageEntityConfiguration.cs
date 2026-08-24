using Stowaway.Domain.Entities.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Infrastructure.Database.Configurations.Storage
{
    public class ItemImageEntityConfiguration : IEntityTypeConfiguration<ItemImageEntity>
    {
        public void Configure(EntityTypeBuilder<ItemImageEntity> builder)
        {
            builder.HasKey(i => i.Id);

            builder.HasOne(i => i.Item)
                .WithMany(i => i.Images)
                .HasForeignKey(i => i.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(i => i.ByteImage).IsRequired();
        }
    }
}
