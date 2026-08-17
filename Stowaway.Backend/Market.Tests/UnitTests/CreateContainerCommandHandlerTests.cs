using FluentValidation;
using Market.Tests.Helpers;
using Stowaway.Application.Modules.Storage.Container.Commands.Create;
using Stowaway.Domain.Entities.Storage;

namespace Market.Tests.UnitTests;

public class CreateContainerCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreatesContainer_WhenWarehouseAndContainerTypeExist()
    {
        await using var db = TestDbContext.Create();
        db.Warehouses.Add(new WarehouseEntity { Id = 1, Name = "W1", Description = "d", City = "c", Address = "a", Capacity = 100, isEnabled = true });
        db.ContainerTypes.Add(new ContainerTypeEntity { Id = 1, MaxItems = 50, MaxContainers = 5, Price = 10m });
        await db.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateContainerCommandHandler(db);

        var containerId = await handler.Handle(new CreateContainerCommand
        {
            Name = "Shelf A",
            WarehouseId = 1,
            ContainerTypeId = 1
        }, CancellationToken.None);

        var created = await db.Containers.FindAsync(containerId);
        Assert.NotNull(created);
        Assert.Equal("Shelf A", created!.Name);
        Assert.Equal(1, created.WarehouseId);
        Assert.Null(created.ParentContainerId);
    }

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenChildContainerTypeIsNotStrictlySmallerThanParent()
    {
        await using var db = TestDbContext.Create();
        db.Warehouses.Add(new WarehouseEntity { Id = 1, Name = "W1", Description = "d", City = "c", Address = "a", Capacity = 100, isEnabled = true });
        var largeType = new ContainerTypeEntity { Id = 1, MaxItems = 100, MaxContainers = 10, Price = 20m };
        db.ContainerTypes.Add(largeType);
        db.Containers.Add(new ContainerEntity { Id = 1, Name = "Parent", WarehouseId = 1, ContainerTypeId = largeType.Id });
        await db.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateContainerCommandHandler(db);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new CreateContainerCommand
            {
                Name = "Same-size child",
                WarehouseId = 1,
                ContainerTypeId = largeType.Id,
                ParentContainerId = 1
            }, CancellationToken.None));
    }
}
