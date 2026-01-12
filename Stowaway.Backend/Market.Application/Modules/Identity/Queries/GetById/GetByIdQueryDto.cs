using Stowaway.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stowaway.Application.Modules.Identity.Queries.GetById
{
    public class GetByIdQueryDto
    {
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Role { get; set; }
        
    }
}