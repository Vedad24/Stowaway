using Stowaway.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Identity.Users.Commands.Create
{
    public class CreateUserCommandHandler(IAppDbContext dbContext, IAppCurrentUser currentUser, IPasswordHasher<UserEntity> hasher) : IRequestHandler<CreateUserCommand, int>
    {
        public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var roleId = request.Role?.Id ?? Role.User;
            if (roleId != Role.User && !currentUser.IsAdmin)
                throw new StowawayUnauthorizedException("Only an Admin can assign a role other than User.");

            UserEntity user = new()
            {
                Email = request.Email,
                PasswordHash = hasher.HashPassword(null!, request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                RoleId = roleId,
                IsEnabled = request.IsEnabled ?? true
            };

            await dbContext.Users.AddAsync(user, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return user.Id;
        }
    }
}
