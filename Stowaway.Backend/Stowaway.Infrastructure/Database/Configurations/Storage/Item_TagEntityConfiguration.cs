using Stowaway.Domain.Entities.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Infrastructure.Database.Configurations.Storage
{
    public class Item_TagEntityConfiguration : IEntityTypeConfiguration<Item_TagEntity>
    {
        
        public void Configure(EntityTypeBuilder<Item_TagEntity> builder)
        {
            builder.HasKey(it => new { it.ItemId, it.TagId});
        }
    }
}
