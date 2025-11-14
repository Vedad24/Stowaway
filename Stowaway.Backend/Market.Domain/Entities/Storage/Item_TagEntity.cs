using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Domain.Entities.Storage
{
    public class Item_TagEntity
    {
        public int ItemId { get; set; }
        public ItemEntity? Item { get; set; }
        
        public int TagId { get; set; }
        public TagEntity? Tag { get; set; }

    }
}
