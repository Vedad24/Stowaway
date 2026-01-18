using Stowaway.Domain.Entities.Identity;

namespace Stowaway.Application.Modules.Identity.Roles.Queries.List
{
    public class ListRolesQueryDto
    {
        public Role roleId {  get; set; }
        public string roleName { get; set; }
    }
}