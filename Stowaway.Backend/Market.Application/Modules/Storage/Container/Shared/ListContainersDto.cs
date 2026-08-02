using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Storage.Container.Shared
{
    public sealed class ListContainersDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public int ContainerTypeId { get; set; }
        public int WarehouseId { get; set; }
        public int? ParentContainerId { get; set; }
        public bool HasChildren { get; set; }
        public double? CanvasX { get; set; }
        public double? CanvasY { get; set; }
    }
}
