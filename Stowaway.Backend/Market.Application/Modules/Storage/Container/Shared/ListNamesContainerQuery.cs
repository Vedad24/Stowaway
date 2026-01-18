using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stowaway.Application.Modules.Storage.Items.Queries.List;

namespace Stowaway.Application.Modules.Storage.Container.Shared
{
    public class ListNamesContainerQuery : BasePagedQuery<SharedContainerDto>
    {
        public string? Search { get; init; }
    }
}
