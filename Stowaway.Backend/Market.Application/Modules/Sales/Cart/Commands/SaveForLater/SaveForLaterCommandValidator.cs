namespace Market.Application.Modules.Sales.Cart.Commands.SaveForLater
{
    public sealed class SaveForLaterCommandValidator : AbstractValidator<SaveForLaterCommand>
    {
        public SaveForLaterCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0);

            RuleFor(x => x.WarehouseId)
                .GreaterThan(0);

            RuleFor(x => x.ContainerType)
                .NotNull();

            RuleFor(x => x.ContainerType.Id)
                .GreaterThan(0)
                .When(x => x.ContainerType is not null);

            RuleFor(x => x.Quantity)
                .GreaterThan(0);
        }
    }
}
