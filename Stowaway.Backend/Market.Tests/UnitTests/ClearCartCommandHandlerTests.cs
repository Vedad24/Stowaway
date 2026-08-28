using Market.Application.Common.Exceptions;
using Market.Application.Modules.Sales.Cart.Commands.ClearCart;
using Market.Domain.Entities.Identity;
using Market.Domain.Entities.Sales;
using Market.Shared.Constants;
using Market.Tests.Helpers;
using Stowaway.Domain.Entities.Identity;
using Stowaway.Domain.Entities.Storage;

namespace Market.Tests.UnitTests;

public class ClearCartCommandHandlerTests
{
    private static async Task SeedUserWithCartItem(TestDbContext db)
    {
        db.Roles.Add(new RoleEntity { Id = Role.User });
        db.Users.Add(new UserEntity { Id = 1, Email = "u@example.com", PasswordHash = "hash", RoleId = Role.User, IsEnabled = true });
        var containerType = new ContainerTypeEntity { Id = 1, MaxItems = 50, MaxContainers = 5, Price = 10m };
        db.ContainerTypes.Add(containerType);
        db.CartItems.Add(new CartItemEntity { UserId = 1, WarehouseId = 1, ContainerType = containerType, Quantity = 1, CartItemStatus = CartItemStatus.InCart });
        await db.SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ThrowsBusinessRuleException_WhenUserIdDoesNotMatchCaller()
    {
        await using var db = TestDbContext.Create();
        await SeedUserWithCartItem(db);

        var handler = new ClearCartCommandHandler(db, new FakeCurrentUser { UserId = 2 });

        await Assert.ThrowsAsync<StowawayBusinessRuleException>(() =>
            handler.Handle(new ClearCartCommand { UserId = 1 }, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Succeeds_WhenCallerHasCartManageAnyPermission()
    {
        await using var db = TestDbContext.Create();
        await SeedUserWithCartItem(db);

        var handler = new ClearCartCommandHandler(db, new FakeCurrentUser { UserId = 2, Permissions = [Permissions.CartManageAny] });

        await handler.Handle(new ClearCartCommand { UserId = 1 }, CancellationToken.None);

        Assert.False(await db.CartItems.AnyAsync(x => x.UserId == 1 && x.CartItemStatus == CartItemStatus.InCart));
    }
}
