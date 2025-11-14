using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace Stowaway.Domain.Entities.Storage
{
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
