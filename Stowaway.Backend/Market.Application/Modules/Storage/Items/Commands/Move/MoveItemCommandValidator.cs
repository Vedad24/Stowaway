using FluentValidation;

namespace Stowaway.Application.Modules.Storage.Items.Commands.Move;
public sealed class MoveItemCommandValidator : AbstractValidator<MoveItemCommand>
{
    public MoveItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id is required.");

        RuleFor(x => x.ContainerId)
            .GreaterThan(0).WithMessage("Container is required.");
    }
}
