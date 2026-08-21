using FluentValidation;

namespace Stowaway.Application.Modules.Storage.Items.Commands.Update;
public sealed class UpdateItemCommandValidator : AbstractValidator<UpdateItemCommand>
{
    public UpdateItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

        RuleFor(x => x.ContainerId)
            .GreaterThan(0).WithMessage("Container is required.");

        RuleFor(x => x.SupplierId)
            .GreaterThan(0).WithMessage("Supplier is required.");
    }
}
