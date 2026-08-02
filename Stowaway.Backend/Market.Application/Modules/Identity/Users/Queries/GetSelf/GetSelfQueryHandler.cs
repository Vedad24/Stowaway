using Stowaway.Domain.Entities.Identity;

namespace Stowaway.Application.Modules.Identity.Users.Queries.GetSelf
{
    public class GetSelfQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
        : IRequestHandler<GetSelfQuery, GetSelfQueryDto>
    {
        public async Task<GetSelfQueryDto> Handle(GetSelfQuery request, CancellationToken cancellationToken)
        {
            if (currentUser.UserId is null)
                throw new StowawayNotFoundException("Current user could not be resolved.");

            UserEntity? ur = await context.Users.FirstOrDefaultAsync(u => u.Id == currentUser.UserId, cancellationToken);
            if (ur == null)
            {
                throw new StowawayNotFoundException($"User with ID : {currentUser.UserId} not found");
            }
            GetSelfQueryDto dto = new()
            {
                Email = ur.Email,
                FirstName = ur.FirstName ?? "[no_name]",
                LastName = ur.LastName ?? "[no_name]",
                Role = new GetSelfQueryDtoRoleDto { Id = (int)(ur.RoleId ?? Role.User) }
            };
            return dto;
        }
    }
}
