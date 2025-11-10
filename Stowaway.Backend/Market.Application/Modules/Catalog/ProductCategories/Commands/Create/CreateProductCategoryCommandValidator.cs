namespace Market.Application.Modules.Catalog.ProductCategories.Commands.Create;

public sealed class CreateProductCategoryCommandValidator
    : AbstractValidator<CreateProductCategoryCommand>
{
    public CreateProductCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(WarehouseEntity.Constraints.NameMaxLength).WithMessage($"Name can be at most {WarehouseEntity.Constraints.NameMaxLength} characters long.");
    }
}