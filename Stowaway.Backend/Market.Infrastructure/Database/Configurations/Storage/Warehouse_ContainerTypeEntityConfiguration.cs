using Stowaway.Domain.Entities.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Infrastructure.Database.Configurations.Storage
{
    public class Warehouse_ContainerTypeEntityConfiguration : IEntityTypeConfiguration<Warehouse_ContainerTypeEntity>
    {
        public void Configure(EntityTypeBuilder<Warehouse_ContainerTypeEntity> builder)
        {
            builder.HasKey(wct => new { wct.WarehouseId, wct.ContainerTypeId });
        }
    }
}
