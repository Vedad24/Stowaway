namespace Stowaway.Application.Modules.Storage.Warehouse.Commands.Update
{
    public sealed class UpdateWarehouseCommandValidator : AbstractValidator<UpdateWarehouseCommand>
    {
        public UpdateWarehouseCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(200).WithMessage("Address must not exceed 200 characters.");

            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than 0.");
        }
    }
}
