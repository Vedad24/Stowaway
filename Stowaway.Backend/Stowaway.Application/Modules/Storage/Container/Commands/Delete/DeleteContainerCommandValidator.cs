using FluentValidation;

namespace Stowaway.Application.Modules.Storage.Container.Commands.Delete;

public sealed class DeleteContainerCommandValidator : AbstractValidator<DeleteContainerCommand>
{
    public DeleteContainerCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id is required.");

        RuleFor(x => x.MoveContentsToContainerId)
            .GreaterThan(0).WithMessage("Target container is not valid.")
            .When(x => x.MoveContentsToContainerId.HasValue);
    }
}
