using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Domain.Entities.Storage
{
    [Table("ContainerStatus", Schema = "Storage")]
    public class ContainerStatusEntity
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
