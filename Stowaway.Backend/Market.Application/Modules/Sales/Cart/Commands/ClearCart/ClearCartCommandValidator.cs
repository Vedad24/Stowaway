namespace Market.Application.Modules.Sales.Cart.Commands.ClearCart
{
    public sealed class ClearCartCommandValidator : AbstractValidator<ClearCartCommand>
    {
        public ClearCartCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0);
        }
    }
}
