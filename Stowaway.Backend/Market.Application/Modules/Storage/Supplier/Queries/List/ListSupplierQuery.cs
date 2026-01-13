using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Storage.Supplier.Queries.List
{
    public class ListSupplierQuery : BasePagedQuery<ListSupplierQueryDto>
    {
        public string? Search { get; init; }

    }
}
