namespace Stowaway.Application.Modules.Sales.Cart.Queries.List
{
    public sealed class ListCartItemsQueryValidator : AbstractValidator<ListCartItemsQuery>
    {
        public ListCartItemsQueryValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0);
        }
    }
}
