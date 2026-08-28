using Market.Application.Common.Exceptions;
using Market.Application.Modules.Sales.Cart.Queries.List;
using Market.Domain.Entities.Identity;
using Market.Domain.Entities.Sales;
using Market.Shared.Constants;
using Market.Tests.Helpers;
using Stowaway.Domain.Entities.Identity;
using Stowaway.Domain.Entities.Storage;

namespace Market.Tests.UnitTests;

public class ListCartItemsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCartItems_WhenRequestedByOwner()
    {
        await using var db = TestDbContext.Create();
        db.Roles.Add(new RoleEntity { Id = Role.User });
        db.Users.Add(new UserEntity { Id = 1, Email = "u@example.com", PasswordHash = "hash", RoleId = Role.User, IsEnabled = true });
        db.Warehouses.Add(new WarehouseEntity { Id = 1, Name = "W1", Description = "d", City = "c", Address = "a", Capacity = 100, isEnabled = true });
        var containerType = new ContainerTypeEntity { Id = 1, MaxItems = 50, MaxContainers = 5, Price = 10m };
        db.ContainerTypes.Add(containerType);
        db.CartItems.Add(new CartItemEntity { UserId = 1, WarehouseId = 1, ContainerType = containerType, Quantity = 1, CartItemStatus = CartItemStatus.InCart });
        await db.SaveChangesAsync(CancellationToken.None);

        var handler = new ListCartItemsQueryHandler(db, new FakeCurrentUser { UserId = 1 });

        var result = await handler.Handle(new ListCartItemsQuery { UserId = 1 }, CancellationToken.None);

        Assert.Single(result.CartItems);
    }

    [Fact]
    public async Task Handle_ThrowsBusinessRuleException_WhenUserIdDoesNotMatchCaller()
    {
        await using var db = TestDbContext.Create();
        db.Roles.Add(new RoleEntity { Id = Role.User });
        db.Users.Add(new UserEntity { Id = 1, Email = "u@example.com", PasswordHash = "hash", RoleId = Role.User, IsEnabled = true });
        await db.SaveChangesAsync(CancellationToken.None);

        var handler = new ListCartItemsQueryHandler(db, new FakeCurrentUser { UserId = 2 });

        await Assert.ThrowsAsync<StowawayBusinessRuleException>(() =>
            handler.Handle(new ListCartItemsQuery { UserId = 1 }, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ReturnsCartItems_WhenCallerHasCartManageAnyPermission()
    {
        await using var db = TestDbContext.Create();
        db.Roles.Add(new RoleEntity { Id = Role.User });
        db.Users.Add(new UserEntity { Id = 1, Email = "u@example.com", PasswordHash = "hash", RoleId = Role.User, IsEnabled = true });
        db.Warehouses.Add(new WarehouseEntity { Id = 1, Name = "W1", Description = "d", City = "c", Address = "a", Capacity = 100, isEnabled = true });
        var containerType = new ContainerTypeEntity { Id = 1, MaxItems = 50, MaxContainers = 5, Price = 10m };
        db.ContainerTypes.Add(containerType);
        db.CartItems.Add(new CartItemEntity { UserId = 1, WarehouseId = 1, ContainerType = containerType, Quantity = 1, CartItemStatus = CartItemStatus.InCart });
        await db.SaveChangesAsync(CancellationToken.None);

        var handler = new ListCartItemsQueryHandler(db, new FakeCurrentUser { UserId = 2, Permissions = [Permissions.CartManageAny] });

        var result = await handler.Handle(new ListCartItemsQuery { UserId = 1 }, CancellationToken.None);

        Assert.Single(result.CartItems);
    }
}
