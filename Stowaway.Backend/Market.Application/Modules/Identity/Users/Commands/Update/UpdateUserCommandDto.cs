using Stowaway.Domain.Entities.Identity;

namespace Stowaway.Application.Modules.Identity.Users.Commands.Update
{
    public class UpdateUserCommandDto
    {
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required RoleEntity Role { get; set; }
        public required bool IsEnabled { get; set; }
    }
}