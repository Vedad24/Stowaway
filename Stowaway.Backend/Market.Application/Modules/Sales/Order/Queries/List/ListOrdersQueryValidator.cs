namespace Stowaway.Application.Modules.Sales.Order.Queries.List
{
    public sealed class ListOrdersQueryValidator : AbstractValidator<ListOrdersQuery>
    {
        public ListOrdersQueryValidator()
        {
            RuleFor(x => x.SearchByUserEmail)
                .MaximumLength(200);

            RuleFor(x => x.SearchByUserName)
                .MaximumLength(200);

            RuleFor(x => x.SearchByWarehouseName)
                .MaximumLength(200);

            RuleFor(x => x.CreateTimeMax)
                .GreaterThanOrEqualTo(x => x.CreateTimeMin)
                .WithMessage("CreateTimeMax must be greater than or equal to CreateTimeMin.")
                .When(x => x.CreateTimeMin.HasValue && x.CreateTimeMax.HasValue);
        }
    }
}
