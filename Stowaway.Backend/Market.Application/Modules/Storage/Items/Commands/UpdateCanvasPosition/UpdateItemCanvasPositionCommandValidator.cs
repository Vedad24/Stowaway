using FluentValidation;

namespace Stowaway.Application.Modules.Storage.Items.Commands.UpdateCanvasPosition;

public sealed class UpdateItemCanvasPositionCommandValidator : AbstractValidator<UpdateItemCanvasPositionCommand>
{
    public UpdateItemCanvasPositionCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id is required.");
    }
}
