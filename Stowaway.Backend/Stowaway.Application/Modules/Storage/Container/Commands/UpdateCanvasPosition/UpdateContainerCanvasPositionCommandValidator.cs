using FluentValidation;

namespace Stowaway.Application.Modules.Storage.Container.Commands.UpdateCanvasPosition;
public sealed class UpdateContainerCanvasPositionCommandValidator : AbstractValidator<UpdateContainerCanvasPositionCommand>
{
    public UpdateContainerCanvasPositionCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id is required.");
    }
}
