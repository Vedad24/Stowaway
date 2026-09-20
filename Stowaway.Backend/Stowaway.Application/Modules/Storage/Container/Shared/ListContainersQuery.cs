using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stowaway.Application.Modules.Storage.Items.Queries.List;

namespace Stowaway.Application.Modules.Storage.Container.Shared
{
    public sealed class ListContainersQuery : BasePagedQuery<ListContainersDto>
    {
        public string? Search { get; init; }
        public int? WarehouseId { get; init; }
        public int? ParentContainerId { get; init; }
        public int? ContainerTypeId { get; init; }
        public int? StatusId { get; init; }
    }
}
