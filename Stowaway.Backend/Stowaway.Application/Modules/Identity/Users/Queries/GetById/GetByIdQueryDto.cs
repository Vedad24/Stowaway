using Stowaway.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stowaway.Application.Modules.Identity.Users.Queries.GetById
{
    public class GetByIdQueryDto
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required GetByIdQueryDtoRoleDto Role { get; set; }

    }

    public class GetByIdQueryDtoRoleDto
    {
        public int Id { get; set; }
    }
}