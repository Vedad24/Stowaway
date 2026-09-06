using Stowaway.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stowaway.Application.Modules.Identity.Users.Queries.GetByMail
{
    public class GetByMailQueryDto
    {
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required Role Role { get; set; }

    }
}