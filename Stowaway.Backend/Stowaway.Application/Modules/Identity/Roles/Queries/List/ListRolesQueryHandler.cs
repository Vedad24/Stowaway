using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Identity.Roles.Queries.List
{
    public class ListRolesQueryHandler(IAppDbContext db) : IRequestHandler<ListRolesQuery, List<ListRolesQueryDto>>
    {
        public async Task<List<ListRolesQueryDto>> Handle(ListRolesQuery request, CancellationToken cancellationToken)
        {
            return await db.Roles.Select(r => new ListRolesQueryDto()
            {
                roleId = r.Id,
                roleName = Enum.GetName(r.Id) ?? "Unknown"
            }).ToListAsync(cancellationToken);
        }
    }
}
