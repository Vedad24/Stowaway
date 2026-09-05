namespace Stowaway.Application.Modules.Identity.Users.Commands.Update
{
    public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email cannot be empty.")
                .EmailAddress().WithMessage("Email is not a valid email address.")
                .When(x => x.Email is not null);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .When(x => x.Password is not null);

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("FirstName cannot be empty.")
                .When(x => x.FirstName is not null);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("LastName cannot be empty.")
                .When(x => x.LastName is not null);

            RuleFor(x => x.Role)
                .Must(role => role!.Id >= 0)
                .WithMessage("Role is not valid.")
                .When(x => x.Role is not null);
        }
    }
}
