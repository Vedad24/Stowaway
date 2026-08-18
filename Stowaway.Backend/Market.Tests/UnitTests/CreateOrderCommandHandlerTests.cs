using Market.Application.Common.Exceptions;
using Market.Tests.Helpers;
using Stowaway.Application.Modules.Sales.Order.Commands.Create;
using Stowaway.Application.Modules.Sales.Order.Shared;
using Stowaway.Domain.Entities.Storage;

namespace Market.Tests.UnitTests;

public class CreateOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_CalculatesDiscountedTotals_WhenContainerTypesExist()
    {
        await using var db = TestDbContext.Create();
        db.Warehouses.Add(new WarehouseEntity { Id = 1, Name = "W1", Description = "d", City = "c", Address = "a", Capacity = 100, isEnabled = true });
        db.ContainerTypes.Add(new ContainerTypeEntity { Id = 1, MaxItems = 50, MaxContainers = 5, Price = 100m });
        await db.SaveChangesAsync(CancellationToken.None);
        db.ChangeTracker.Clear();

        var handler = new CreateOrderCommandHandler(db);

        var dto = await handler.Handle(new CreateOrderCommand
        {
            UserId = 1,
            OrderItems = new List<SharedOrderCommandContainerType>
            {
                new() { ContainerTypeId = 1, WarehouseId = 1, Quantity = 2 }
            }
        }, CancellationToken.None);

        var created = await db.Orders.FindAsync(dto.OrderId);
        Assert.NotNull(created);
        Assert.Equal(200m, created!.Subtotal);
        Assert.Equal(190m, created.Total);
    }

    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenContainerTypeDoesNotExist()
    {
        await using var db = TestDbContext.Create();
        db.Warehouses.Add(new WarehouseEntity { Id = 1, Name = "W1", Description = "d", City = "c", Address = "a", Capacity = 100, isEnabled = true });
        await db.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateOrderCommandHandler(db);

        await Assert.ThrowsAsync<StowawayNotFoundException>(() =>
            handler.Handle(new CreateOrderCommand
            {
                UserId = 1,
                OrderItems = new List<SharedOrderCommandContainerType>
                {
                    new() { ContainerTypeId = 999, WarehouseId = 1, Quantity = 1 }
                }
            }, CancellationToken.None));
    }
}
