using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Domain.Entities.Storage
{
    public class ContainerEntity
    {
        public int Id { get; set; }
        public int ContainerTypeId { get; set; }
        public ContainerTypeEntity? ContainerType { get; set; }
        public int? ParentContainerId { get; set; }
        public ContainerEntity? ParentContainer { get; set; }
        public int WarehouseId { get; set; }
        public WarehouseEntity? Warehouse { get; set; }

    }
}
