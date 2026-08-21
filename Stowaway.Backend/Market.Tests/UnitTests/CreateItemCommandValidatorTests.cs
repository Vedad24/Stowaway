using FluentValidation.TestHelper;
using Stowaway.Application.Modules.Storage.Items.Commands.Create;

namespace Market.Tests.UnitTests;

public class CreateItemCommandValidatorTests
{
    private readonly CreateItemCommandValidator _validator = new();

    private static CreateItemCommand ValidCommand(
        string name = "Widget",
        string description = "A widget",
        int quantity = 1,
        int containerId = 1,
        int supplierId = 1) => new()
    {
        Name = name,
        Description = description,
        Quantity = quantity,
        ContainerId = containerId,
        SupplierId = supplierId,
    };

    [Fact]
    public void Should_HaveError_WhenNameIsEmpty()
    {
        var result = _validator.TestValidate(ValidCommand(name: ""));

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_HaveError_WhenNameExceedsMaxLength()
    {
        var result = _validator.TestValidate(ValidCommand(name: new string('a', 101)));

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_HaveError_WhenDescriptionExceedsMaxLength()
    {
        var result = _validator.TestValidate(ValidCommand(description: new string('a', 1001)));

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_HaveError_WhenQuantityIsNotPositive(int quantity)
    {
        var result = _validator.TestValidate(ValidCommand(quantity: quantity));

        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_HaveError_WhenContainerIdIsNotPositive(int containerId)
    {
        var result = _validator.TestValidate(ValidCommand(containerId: containerId));

        result.ShouldHaveValidationErrorFor(x => x.ContainerId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_HaveError_WhenSupplierIdIsNotPositive(int supplierId)
    {
        var result = _validator.TestValidate(ValidCommand(supplierId: supplierId));

        result.ShouldHaveValidationErrorFor(x => x.SupplierId);
    }

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        var result = _validator.TestValidate(ValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }
}
