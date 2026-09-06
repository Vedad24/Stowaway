using Stowaway.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Identity.Users.Commands.Create
{
    public class CreateUserCommand : IRequest<int>
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public RoleEntity? Role { get; set; }
        public bool? IsEnabled { get; set; }
    }
}
