namespace Stowaway.Application.Modules.Sales.Order.Commands.Create
{
    public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0);

            RuleFor(x => x.OrderItems)
                .NotNull()
                .NotEmpty().WithMessage("At least one order item is required.");

            RuleForEach(x => x.OrderItems).ChildRules(item =>
            {
                item.RuleFor(i => i.ContainerTypeId).GreaterThan(0);
                item.RuleFor(i => i.WarehouseId).GreaterThan(0);
                item.RuleFor(i => i.Quantity).GreaterThan(0);
            });
        }
    }
}
