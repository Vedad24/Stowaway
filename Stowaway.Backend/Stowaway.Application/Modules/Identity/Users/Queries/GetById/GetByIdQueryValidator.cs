namespace Stowaway.Application.Modules.Identity.Users.Queries.GetById
{
    public sealed class GetByIdQueryValidator : AbstractValidator<GetByIdQuery>
    {
        public GetByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
