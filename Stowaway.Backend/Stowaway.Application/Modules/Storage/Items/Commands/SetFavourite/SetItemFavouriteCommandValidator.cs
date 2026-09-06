using FluentValidation;

namespace Stowaway.Application.Modules.Storage.Items.Commands.SetFavourite;

public sealed class SetItemFavouriteCommandValidator : AbstractValidator<SetItemFavouriteCommand>
{
    public SetItemFavouriteCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id is required.");
    }
}
