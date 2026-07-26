using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stowaway.Application.Modules.Storage.Items.Queries.List;

namespace Stowaway.Application.Modules.Storage.Container.Shared
{
    public class ListContainersQuery : IRequest<List<ListContainersDto>>
    {
        public string? Search { get; init; }
        public int? WarehouseId { get; init; }
        public int? ParentContainerId { get; init; }
    }
}
