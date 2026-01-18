using Stowaway.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Identity.Users.Queries.List
{
    public class ListUserQuery : BasePagedQuery<ListUserQueryDto>
    {

        public string? Search { get; init; }
        public Role? RoleId { get; init; }
    }
}
