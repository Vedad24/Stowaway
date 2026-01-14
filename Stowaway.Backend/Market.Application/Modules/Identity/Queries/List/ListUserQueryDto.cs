using Stowaway.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stowaway.Application.Modules.Identity.Queries.List
{
    public class ListUserQueryDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Role { get; set; }
        public bool IsEnabled { get; set; }
    }
}