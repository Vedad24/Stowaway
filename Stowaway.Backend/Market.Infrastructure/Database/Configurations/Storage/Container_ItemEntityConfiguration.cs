using Stowaway.Domain.Entities.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Infrastructure.Database.Configurations.Storage
{
    public class Container_ItemEntityConfiguration : IEntityTypeConfiguration<Container_ItemEntity>
    {
        public void Configure(EntityTypeBuilder<Container_ItemEntity> builder)
        {
            builder.HasKey(ci => new { ci.ItemId, ci.ContainerId });
        }
    }
}
