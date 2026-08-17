using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Storage.Items.Queries.List
{
    public sealed class ListItemQuery : BasePagedQuery<ListItemQueryDto>
    {
        public string? Search { get; init; }
        public int? ContainerId { get; init; }
    }
}
