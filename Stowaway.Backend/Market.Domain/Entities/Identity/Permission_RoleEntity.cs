using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Domain.Entities.Identity
{
    [Table("Permission_Role", Schema = "Identity")]
    public class Permission_RoleEntity
    {
        public int RoleId { get; set; }
        public RoleEntity Role { get; set; }
        public int PermissionId { get; set; }
        public PermissionEntity Permission { get; set; }

    }
}
