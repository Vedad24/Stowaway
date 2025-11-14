using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Domain.Entities.Storage
{
    [Table("Container_Item", Schema ="Storage")]
    public class Container_ItemEntity
    {
        public int ContainerId { get; set; }
        public ContainerEntity? Container { get; set; }
        public int ItemId { get; set; }
        public ItemEntity? Item { get; set; }
        public int Quantity { get; set; }
    }
}
