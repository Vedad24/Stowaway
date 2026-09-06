using FluentValidation;

namespace Stowaway.Application.Modules.Storage.Container.Commands.Move;
public sealed class MoveContainerCommandValidator : AbstractValidator<MoveContainerCommand>
{
    public MoveContainerCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id is required.");

        RuleFor(x => x.ParentContainerId)
            .GreaterThan(0).WithMessage("Parent container is required.");
    }
}
