using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Identity.Users.Queries.GetById
{
    public class GetByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetByIdQuery, GetByIdQueryDto>
    {
        public async Task<GetByIdQueryDto> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            UserEntity? ur = await context.Users.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
            if (ur == null)
            {
                throw new StowawayNotFoundException($"User with ID : {request.Id} not found");
            }
            GetByIdQueryDto dto = new()
            {
                Id = ur.Id,
                Email = ur.Email,
                FirstName = ur.FirstName ?? "[no_name]",
                LastName = ur.LastName ?? "[no_name]",
                Role = new GetByIdQueryDtoRoleDto{Id = (int) (ur.RoleId ?? Domain.Entities.Identity.Role.User) }
            };
            return dto;
        }
    }
}
