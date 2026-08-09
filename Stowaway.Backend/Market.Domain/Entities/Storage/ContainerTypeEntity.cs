using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Domain.Entities.Storage
{
    [Table("ContainerType", Schema = "Storage")]

    public class ContainerTypeEntity
    {
        public int Id { get; set; }
        public int MaxItems { get; set; }
        public int MaxContainers { get; set; }
        public decimal Price { get; set; }


        public override string ToString()
        {
            var sizeName = MaxItems switch
            {
                10 => "Small",
                50 => "Medium",
                100 => "Large",
                500 => "Storage Box",
                _ => $"i{MaxItems} - c{MaxContainers}",
            };

            return $"{sizeName} — {MaxItems} items";
        }
    }
}
