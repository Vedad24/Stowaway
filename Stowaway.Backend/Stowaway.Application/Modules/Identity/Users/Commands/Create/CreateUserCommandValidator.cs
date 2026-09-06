using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Stowaway.Application.Abstractions;

namespace Stowaway.Application.Modules.Identity.Users.Commands.Create;

/// <summary>
/// FluentValidation validator for <see cref="CreateUserCommand"/>.
/// </summary>
public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator(IAppDbContext dbContext)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is not a valid email address.")
            .MustAsync(async (email, cancellation) =>
                !await dbContext.Users.AnyAsync(u => u.Email == email, cancellation))
            .WithMessage("A user with this email already exists.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("FirstName is required.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("LastName is required.");

        RuleFor(x => x.Role)
            .Must(role => role == null || role.Id >= 0)
            .WithMessage("Role is not valid.")
            .When(x => x.Role is not null);
    }
}
