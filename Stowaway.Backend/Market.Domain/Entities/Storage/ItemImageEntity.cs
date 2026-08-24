using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Domain.Entities.Storage
{
    [Table("ItemImage", Schema = "Storage")]

    public class ItemImageEntity
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public ItemEntity? Item { get; set; }
        public byte[] ByteImage { get; set; }
        public int SortOrder { get; set; }
    }
}
