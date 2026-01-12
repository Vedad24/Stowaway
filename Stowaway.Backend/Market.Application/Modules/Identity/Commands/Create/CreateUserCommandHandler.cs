using Stowaway.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Identity.Commands.Create
{
    public class CreateUserCommandHandler(IAppDbContext dbContext) : IRequestHandler<CreateUserCommand, int>
    {
        public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (!isValid(request))
                throw new StowawayConflictException("Request not valid");

            var hasher = new PasswordHasher<UserEntity>();
            UserEntity user = new()
            {
                Email = request.Email,
                PasswordHash = hasher.HashPassword(null!, request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                RoleId = request.Role == null ? Role.User : request.Role.Id,
                IsEnabled = request.IsEnabled ?? true
            };

            await dbContext.Users.AddAsync(user, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return user.Id;

        }

        private bool isValid(CreateUserCommand request)
        {
            bool noName = string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName);
            return !dbContext.Users.Any(u => u.Email == request.Email) && !noName;
        }
    }
}
