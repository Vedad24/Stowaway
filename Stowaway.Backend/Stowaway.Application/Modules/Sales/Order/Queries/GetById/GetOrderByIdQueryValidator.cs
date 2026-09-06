namespace Stowaway.Application.Modules.Sales.Order.Queries.GetById
{
    public sealed class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
    {
        public GetOrderByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
