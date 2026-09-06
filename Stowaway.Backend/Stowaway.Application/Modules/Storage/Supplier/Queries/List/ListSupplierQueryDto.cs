using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Storage.Supplier.Queries.List
{
    public class ListSupplierQueryDto
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public required string Description { get; init; }
        public required string Address { get; init; }
        public required int TotalDeliveries { get; init; }
        public required int FailedDeliveries { get; init; }
    }
}
