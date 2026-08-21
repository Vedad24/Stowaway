using FluentValidation;

namespace Stowaway.Application.Modules.Storage.Container.Commands.Create;

public sealed class CreateContainerCommandValidator : AbstractValidator<CreateContainerCommand>
{
    public CreateContainerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.ContainerTypeId)
            .GreaterThan(0).WithMessage("Container type is required.");

        RuleFor(x => x.WarehouseId)
            .GreaterThan(0).WithMessage("Warehouse is required.");

        RuleFor(x => x.ParentContainerId)
            .GreaterThan(0).WithMessage("Parent container is not valid.")
            .When(x => x.ParentContainerId.HasValue);
    }
}
