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
        public string? Address { get; init; }
        public int? MinTotalDeliveries { get; init; }
        public int? MaxTotalDeliveries { get; init; }
        public int? MinFailedDeliveries { get; init; }
        public int? MaxFailedDeliveries { get; init; }
        public double? MinFailureRate { get; init; }
    }
}
