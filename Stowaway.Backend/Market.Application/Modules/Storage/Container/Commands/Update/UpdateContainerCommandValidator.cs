using FluentValidation;

namespace Stowaway.Application.Modules.Storage.Container.Commands.Update;
public sealed class UpdateContainerCommandValidator : AbstractValidator<UpdateContainerCommand>
{
    public UpdateContainerCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.ContainerTypeId)
            .GreaterThan(0).WithMessage("Container type is required.");
    }
}
