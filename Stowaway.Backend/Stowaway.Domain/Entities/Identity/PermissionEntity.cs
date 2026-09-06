using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Domain.Entities.Identity
{
   
    [Table("Permission", Schema = "Identity")]

    public class PermissionEntity
    {
        public int Id { get; set; }
        public required string Description { get; set; }
    }
}
