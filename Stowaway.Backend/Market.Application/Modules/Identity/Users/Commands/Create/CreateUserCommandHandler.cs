using Stowaway.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Stowaway.Application.Modules.Identity.Users.Commands.Create
{
    public class CreateUserCommandHandler(IAppDbContext dbContext) : IRequestHandler<CreateUserCommand, int>
    {
        public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            Validate(request);

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

        private void Validate(CreateUserCommand request)
        {
            // Basic validation: email must be a valid email address
            var emailAttr = new EmailAddressAttribute();
            if (!emailAttr.IsValid(request.Email))
                throw new FluentValidation.ValidationException("Email is not a valid email address");

            bool noName = string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName);
            if (noName)
                throw new FluentValidation.ValidationException("FirstName and LastName must be provided");

            if (dbContext.Users.Any(u => u.Email == request.Email))
                throw new StowawayConflictException("Request not valid");
        }
    }
}
