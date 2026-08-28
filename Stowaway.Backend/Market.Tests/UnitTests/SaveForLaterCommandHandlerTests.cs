using Market.Application.Common.Exceptions;
using Market.Application.Modules.Sales.Cart.Commands.SaveForLater;
using Market.Shared.Constants;
using Market.Tests.Helpers;
using Stowaway.Domain.Entities.Storage;

namespace Market.Tests.UnitTests;

public class SaveForLaterCommandHandlerTests
{
    [Fact]
    public async Task Handle_ThrowsBusinessRuleException_WhenUserIdDoesNotMatchCaller()
    {
        await using var db = TestDbContext.Create();
        db.Warehouses.Add(new WarehouseEntity { Id = 1, Name = "W1", Description = "d", City = "c", Address = "a", Capacity = 100, isEnabled = true });
        var containerType = new ContainerTypeEntity { Id = 1, MaxItems = 50, MaxContainers = 5, Price = 10m };
        db.ContainerTypes.Add(containerType);
        await db.SaveChangesAsync(CancellationToken.None);

        var handler = new SaveForLaterCommandHandler(db, new FakeCurrentUser { UserId = 2 });

        await Assert.ThrowsAsync<StowawayBusinessRuleException>(() =>
            handler.Handle(new SaveForLaterCommand
            {
                UserId = 1,
                WarehouseId = 1,
                ContainerType = containerType,
                Quantity = 1
            }, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Succeeds_WhenCallerHasCartManageAnyPermission()
    {
        await using var db = TestDbContext.Create();
        db.Warehouses.Add(new WarehouseEntity { Id = 1, Name = "W1", Description = "d", City = "c", Address = "a", Capacity = 100, isEnabled = true });
        var containerType = new ContainerTypeEntity { Id = 1, MaxItems = 50, MaxContainers = 5, Price = 10m };
        db.ContainerTypes.Add(containerType);
        await db.SaveChangesAsync(CancellationToken.None);

        var handler = new SaveForLaterCommandHandler(db, new FakeCurrentUser { UserId = 2, Permissions = [Permissions.CartManageAny] });

        await handler.Handle(new SaveForLaterCommand
        {
            UserId = 1,
            WarehouseId = 1,
            ContainerType = containerType,
            Quantity = 1
        }, CancellationToken.None);

        Assert.True(await db.CartItems.AnyAsync(x => x.UserId == 1));
    }
}
