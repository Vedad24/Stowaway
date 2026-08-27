using FluentValidation.TestHelper;
using Stowaway.Application.Modules.Storage.Container.Commands.Create;

namespace Market.Tests.UnitTests;

public class CreateContainerCommandValidatorTests
{
    private readonly CreateContainerCommandValidator _validator = new();

    [Fact]
    public void Should_HaveError_WhenNameIsEmpty()
    {
        var result = _validator.TestValidate(new CreateContainerCommand
        {
            Name = "",
            ContainerTypeId = 1,
            WarehouseId = 1,
        });

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_HaveError_WhenNameExceedsMaxLength()
    {
        var result = _validator.TestValidate(new CreateContainerCommand
        {
            Name = new string('a', 101),
            ContainerTypeId = 1,
            WarehouseId = 1,
        });

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_HaveError_WhenContainerTypeIdIsNotPositive(int containerTypeId)
    {
        var result = _validator.TestValidate(new CreateContainerCommand
        {
            Name = "Shelf A",
            ContainerTypeId = containerTypeId,
            WarehouseId = 1,
        });

        result.ShouldHaveValidationErrorFor(x => x.ContainerTypeId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_HaveError_WhenWarehouseIdIsNotPositive(int warehouseId)
    {
        var result = _validator.TestValidate(new CreateContainerCommand
        {
            Name = "Shelf A",
            ContainerTypeId = 1,
            WarehouseId = warehouseId,
        });

        result.ShouldHaveValidationErrorFor(x => x.WarehouseId);
    }

    [Fact]
    public void Should_HaveError_WhenParentContainerIdIsNotPositive()
    {
        var result = _validator.TestValidate(new CreateContainerCommand
        {
            Name = "Shelf A",
            ContainerTypeId = 1,
            WarehouseId = 1,
            ParentContainerId = 0,
        });

        result.ShouldHaveValidationErrorFor(x => x.ParentContainerId);
    }

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        var result = _validator.TestValidate(new CreateContainerCommand
        {
            Name = "Shelf A",
            ContainerTypeId = 1,
            WarehouseId = 1,
            ParentContainerId = 5,
        });

        result.ShouldNotHaveAnyValidationErrors();
    }
}
