using FluentValidation;

namespace Stowaway.Application.Modules.Storage.Container.Commands.UpdateStatus;
public sealed class UpdateContainerStatusCommandValidator : AbstractValidator<UpdateContainerStatusCommand>
{
    public UpdateContainerStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id is required.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.");
    }
}
