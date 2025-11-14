using Stowaway.Domain.Entities.Storage.StorageIdentity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Infrastructure.Database.Configurations.Storage.StorageIdentity
{
    public class Warehouse_UserEntityConfiguration : IEntityTypeConfiguration<Warehouse_UserEntity>
    {
        public void Configure(EntityTypeBuilder<Warehouse_UserEntity> builder)
        {
            builder.HasKey(wu => new { wu.WarehouseId, wu.UserId, wu.PriviledgeGroupId });
        }
    }
}
