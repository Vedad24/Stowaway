using System;
using System.Collections.Generic;
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
    public class RoleEntity
    {
        public Role Id { get; set; }
    }
}
