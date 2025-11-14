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

    }
}
