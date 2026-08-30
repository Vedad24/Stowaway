namespace Stowaway.Application.Modules.Identity.Users.Queries.GetByMail
{
    public sealed class GetByMailQueryValidator : AbstractValidator<GetByMailQuery>
    {
        public GetByMailQueryValidator()
        {
            RuleFor(x => x.Mail)
                .NotEmpty().WithMessage("Mail is required.")
                .EmailAddress().WithMessage("Mail is not a valid email address.");
        }
    }
}
