
using Microsoft.Extensions.Configuration;
using Stowaway.Domain.Entities.Identity;

namespace Stowaway.Application.Modules.Identity.Users.Commands.Update
{
    public class UpdateUserCommandHandler(IAppDbContext context, IAppCurrentUser currentUser, IPasswordHasher<UserEntity> hasher) : IRequestHandler<UpdateUserCommand, UpdateUserCommandDto>
    {
        public async Task<UpdateUserCommandDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken)
                ?? throw new StowawayNotFoundException($"User with id {request.Id} not found.");
            user.Email = request.Email ?? user.Email;
            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;
            user.IsEnabled = request.IsEnabled ?? user.IsEnabled;
            if (!string.IsNullOrEmpty(request.Password))
                user.PasswordHash = hasher.HashPassword(user, request.Password);
            if (request.Role is not null)
            {
                if (!context.Roles.Any(r => r.Id == request.Role.Id))
                    throw new StowawayNotFoundException($"Role doesn't exist -> RoleId {request.Role.Id}");
                if (request.Role.Id == Role.Admin && !currentUser.IsAdmin)
                    throw new StowawayUnauthorizedException("Only an Admin can assign the Admin role.");
                context.Roles.Attach(request.Role);
                user.Role = request.Role;
            }
            await context.SaveChangesAsync(cancellationToken);
            return new UpdateUserCommandDto()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsEnabled = user.IsEnabled,
                Role = user.Role
            };
        }
    }
}