namespace Stowaway.Application.Modules.Identity.Users.Commands.UpdateSelf;

/// <summary>
/// FluentValidation validator for <see cref="UpdateSelfCommand"/>.
/// </summary>
public sealed class UpdateSelfCommandValidator : AbstractValidator<UpdateSelfCommand>
{
    public UpdateSelfCommandValidator(IAppDbContext dbContext, IAppCurrentUser currentUser)
    {
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email is not a valid email address.")
            .MustAsync(async (email, cancellation) =>
                !await dbContext.Users.AnyAsync(u => u.Email == email && u.Id != currentUser.UserId, cancellation))
            .WithMessage("A user with this email already exists.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}
