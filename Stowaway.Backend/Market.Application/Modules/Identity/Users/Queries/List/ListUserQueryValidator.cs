namespace Stowaway.Application.Modules.Identity.Users.Queries.List
{
    public sealed class ListUserQueryValidator : AbstractValidator<ListUserQuery>
    {
        public ListUserQueryValidator()
        {
            RuleFor(x => x.Search)
                .MaximumLength(200);

            RuleFor(x => x.RoleId)
                .IsInEnum()
                .When(x => x.RoleId.HasValue);
        }
    }
}
