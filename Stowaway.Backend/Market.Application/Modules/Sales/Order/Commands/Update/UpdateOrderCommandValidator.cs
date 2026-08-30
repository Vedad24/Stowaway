namespace Stowaway.Application.Modules.Sales.Order.Commands.Update
{
    public sealed class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.allContainerTypes)
                .NotNull()
                .NotEmpty().WithMessage("At least one order item is required.");

            RuleForEach(x => x.allContainerTypes).ChildRules(item =>
            {
                item.RuleFor(i => i.ContainerTypeId).GreaterThan(0);
                item.RuleFor(i => i.WarehouseId).GreaterThan(0);
                item.RuleFor(i => i.Quantity).GreaterThan(0);
            });
        }
    }
}
