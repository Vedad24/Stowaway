using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Storage.Supplier.Commands.Create
{
    public class CreateSupplierCommand : IRequest<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int TotalDeliveries { get; set; }
        public int FailedDeliveries { get; set; }
    }
}
