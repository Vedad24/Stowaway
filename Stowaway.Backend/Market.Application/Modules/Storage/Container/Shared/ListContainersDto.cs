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
        public string ContainerTypeName { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int? ParentContainerId { get; set; }
        public bool HasChildren { get; set; }
        public double? CanvasX { get; set; }
        public double? CanvasY { get; set; }
        public int MaxItems { get; set; }
        public int MaxContainers { get; set; }
        public int ItemQuantityUsed { get; set; }
        public int ContainerCountUsed { get; set; }
        public SharedContainerStatusDto? CurrentStatus { get; set; }
    }
}
