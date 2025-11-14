using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Domain.Entities.Storage
{
    [Table("Warehouse", Schema = "Storage")]

    public class WarehouseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
