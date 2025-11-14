using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Domain.Entities.Storage
{
    public class SupplierEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TotalDeliveries { get; set; }
        public int FailedDeliveries { get; set; }

    }
}
