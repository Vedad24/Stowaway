using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Stowaway.Domain.Entities.Storage
{
    [Table("Item", Schema = "Storage")]

    public class ItemEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[]? ByteImage { get; set; } //Image....

        public int SupplierId { get; set; }
        public SupplierEntity? Supplier { get; set; }


    }
}
