using Stowaway.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Identity.Users.Queries.GetByMail
{
    public class GetByMailQueryHandler(IAppDbContext context) : IRequestHandler<GetByMailQuery, GetByMailQueryDto>
    {
        public async Task<GetByMailQueryDto> Handle(GetByMailQuery request, CancellationToken cancellationToken)
        {
            UserEntity? ur = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Mail, cancellationToken);
            if (ur == null)
            {
                throw new StowawayNotFoundException($"User with ID : {request.Mail} not found");
            }
            GetByMailQueryDto dto = new()
            {
                Email = ur.Email,
                FirstName = ur.FirstName ?? "[no_name]",
                LastName = ur.LastName ?? "[no_name]",
                Role = ur.RoleId ?? Role.User
            };
            return dto;
        }
    }
}
