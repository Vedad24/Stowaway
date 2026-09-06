using Stowaway.Domain.Entities.Storage.StorageIdentity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Infrastructure.Database.Configurations.Storage.StorageIdentity
{
    public class PriviledgeGroup_PriviledgeEntityConfiguration : IEntityTypeConfiguration<PriviledgeGroup_PriviledgeEntity>
    {
        public void Configure(EntityTypeBuilder<PriviledgeGroup_PriviledgeEntity> builder)
        {
            builder.HasKey(pp => new { pp.PriviledgeGroupId, pp.PriviledgeId });
        }
    }
}
