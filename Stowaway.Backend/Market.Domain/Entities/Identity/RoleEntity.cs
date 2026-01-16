using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Domain.Entities.Identity
{
    public enum Role
    {
        User,
        Admin,
    }
    [Table("Role", Schema = "Identity")]

    public class RoleEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Role Id { get; set; }
    }
}
