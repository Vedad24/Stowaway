using Stowaway.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Identity.Users.Commands.Update
{
    public class UpdateUserCommand : IRequest<UpdateUserCommandDto>
    {
        public required int Id { get; set; }
        public required string? Email { get; set; }
        public required string? FirstName { get; set; }
        public required string? LastName { get; set; }
        public required RoleEntity? Role { get; set; }
        public required bool? IsEnabled { get; set; }
    }
}
