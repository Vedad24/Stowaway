using FluentValidation;
using Market.Tests.Helpers;
using Stowaway.Application.Modules.Storage.Items.Commands.Create;
using Stowaway.Domain.Entities.Storage;

namespace Market.Tests.UnitTests;

public class CreateItemCommandHandlerTests
{
    private static (WarehouseEntity warehouse, ContainerTypeEntity containerType, ContainerEntity container, SupplierEntity supplier) SeedStorage(
        TestDbContext db, int maxItems = 10)
    {
        var warehouse = new WarehouseEntity { Id = 1, Name = "W1", Description = "d", City = "c", Address = "a", Capacity = 100, isEnabled = true };
        var containerType = new ContainerTypeEntity { Id = 1, MaxItems = maxItems, MaxContainers = 5, Price = 10m };
        var container = new ContainerEntity { Id = 1, Name = "Shelf A", WarehouseId = 1, ContainerTypeId = containerType.Id };
        var supplier = new SupplierEntity { Id = 1, Name = "Acme", Description = "d", Address = "a" };

        db.Warehouses.Add(warehouse);
        db.ContainerTypes.Add(containerType);
        db.Containers.Add(container);
        db.Suppliers.Add(supplier);

        return (warehouse, containerType, container, supplier);
    }

    [Fact]
    public async Task Handle_CreatesItem_AndAttachesRequestedTags()
    {
        await using var db = TestDbContext.Create();
        var (_, _, container, supplier) = SeedStorage(db);
        var tag1 = new TagEntity { Id = 1, Name = "Fragile" };
        var tag2 = new TagEntity { Id = 2, Name = "Cold storage" };
        db.Tags.AddRange(tag1, tag2);
        await db.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateItemCommandHandler(db);

        var itemId = await handler.Handle(new CreateItemCommand
        {
            Name = "Widget",
            Description = "A widget",
            Quantity = 3,
            ContainerId = container.Id,
            SupplierId = supplier.Id,
            TagIds = new List<int> { tag1.Id, tag2.Id }
        }, CancellationToken.None);

        var created = await db.Item.FindAsync(itemId);
        Assert.NotNull(created);
        Assert.Equal("Widget", created!.Name);
        Assert.Equal(3, created.Quantity);
        Assert.Equal(container.Id, created.ContainerId);

        var tagLinks = db.ItemTags.Where(t => t.ItemId == itemId).ToList();
        Assert.Equal(2, tagLinks.Count);
        Assert.Contains(tagLinks, t => t.TagId == tag1.Id);
        Assert.Contains(tagLinks, t => t.TagId == tag2.Id);
    }

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenQuantityExceedsContainerCapacity()
    {
        await using var db = TestDbContext.Create();
        var (_, _, container, supplier) = SeedStorage(db, maxItems: 5);
        db.Item.Add(new ItemEntity { Name = "Existing", Description = "d", Quantity = 4, ContainerId = container.Id, SupplierId = supplier.Id });
        await db.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateItemCommandHandler(db);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new CreateItemCommand
            {
                Name = "Overflow",
                Description = "d",
                Quantity = 3,
                ContainerId = container.Id,
                SupplierId = supplier.Id
            }, CancellationToken.None));
    }
}
