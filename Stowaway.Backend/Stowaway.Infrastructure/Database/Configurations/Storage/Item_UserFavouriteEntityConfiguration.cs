using Stowaway.Domain.Entities.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Infrastructure.Database.Configurations.Storage
{
    public class Item_UserFavouriteEntityConfiguration : IEntityTypeConfiguration<Item_UserFavouriteEntity>
    {

        public void Configure(EntityTypeBuilder<Item_UserFavouriteEntity> builder)
        {
            builder.HasKey(f => new { f.ItemId, f.UserId });

            builder.HasOne(f => f.Item)
                .WithMany()
                .HasForeignKey(f => f.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
