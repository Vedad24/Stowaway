using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Domain.Entities.Storage
{
    [Table("Supplier", Schema = "Storage")]

    public class SupplierEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TotalDeliveries { get; set; }
        public int FailedDeliveries { get; set; }

    }
}
