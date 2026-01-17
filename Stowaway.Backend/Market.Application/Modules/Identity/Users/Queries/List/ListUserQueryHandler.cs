using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Identity.Users.Queries.List
{
    public class ListUserQueryHandler(IAppDbContext context) : IRequestHandler<ListUserQuery, PageResult<ListUserQueryDto>>
    {
        public async Task<PageResult<ListUserQueryDto>> Handle(ListUserQuery request, CancellationToken cancellationToken)
        {
            var users = context.Users.AsNoTracking();
            if (request.Search is not null)
            {
                users = users.Where(u => (u.FirstName ?? "").Contains(request.Search) || (u.LastName ?? "").Contains(request.Search) || (u.Email ?? "").Contains(request.Search)).AsNoTracking();
            }
            if (request.RoleId is not null)
            {
                users = users.Where(u => (u.RoleId ?? 0) == request.RoleId).AsNoTracking();
            }
            var result = users.Select(u => new ListUserQueryDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                IsEnabled = u.IsEnabled
            }
            );
            return await PageResult<ListUserQueryDto>.FromQueryableAsync(result, request.Paging, cancellationToken);
        }
    }
}
