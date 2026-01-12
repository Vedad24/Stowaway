using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Storage.Supplier.Shared
{
    public sealed class SharedSupplierDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required int TotalDeliveries { get; set; }
        public required int FailedDeliveries { get; set; }
    }
}
