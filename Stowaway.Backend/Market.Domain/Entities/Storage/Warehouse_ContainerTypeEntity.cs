using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Domain.Entities.Storage
{
    [Table("Warehouse_ContainerType", Schema = "Storage")]
    public class Warehouse_ContainerTypeEntity
    {
        public int WarehouseId { get; set; }
        public WarehouseEntity Warehouse { get; set; }
        public int ContainerTypeId { get; set; }
        public ContainerTypeEntity ContainerType { get; set; }
        public int Max { get; set; }
        public int Min { get; set; }

    }
}
