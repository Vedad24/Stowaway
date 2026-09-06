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
        public int? SupplierId { get; init; }
        public List<int>? TagIds { get; init; }
        public int? MinQuantity { get; init; }
        public int? MaxQuantity { get; init; }
        public bool? FavouritesOnly { get; init; }
    }
}
