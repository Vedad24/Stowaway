using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Stowaway.Application.Modules.Identity.Users.Queries.GetSelf;
using Stowaway.Application.Modules.Sales.Cart.Commands.AddToCart;
using Stowaway.Application.Modules.Sales.Cart.Commands.SaveForLater;
using Stowaway.Application.Modules.Sales.Order.Commands.Create;
using Stowaway.Application.Modules.Sales.Order.Queries.GetById;
using Stowaway.Application.Modules.Sales.Order.Shared;
using Stowaway.Domain.Entities.Sales;
using Stowaway.Domain.Entities.Storage;
using Xunit;

namespace Stowaway.Tests;

// End-to-end HTTP flows through the real MVC pipeline (auth, [HasPermission], MediatR,
// EF InMemory), as opposed to the handler-level UnitTests which bypass all of that.
public class CartOrderFlowTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly CustomWebApplicationFactory<Program> _factory;

    public CartOrderFlowTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    // The default seeded "test" user (Role.User) has no Cart/Order permissions at all -
    // StaticDataSeeder only grants Cart.Manage/Order.* to Manager and Admin - so these
    // flows must authenticate as the manager account instead.
    private async Task<(HttpClient Client, int UserId)> GetManagerClientAsync()
    {
        var client = await _factory.GetAuthenticatedClientAsync("manager@market.local", "Manager123!");
        var self = await client.GetFromJsonAsync<GetSelfQueryDto>("User/me", JsonOptions);
        return (client, self!.UserId);
    }

    [Fact]
    public async Task CartFlow_AddMergesQuantity_SaveForLaterMovesStatus_ClearOnlyRemovesInCartItems()
    {
        var (client, userId) = await GetManagerClientAsync();

        // Seeded ContainerType id 1 (0.5/unit) and Warehouse id 1 ("Main storage room") -
        // see StaticDataSeeder.SeedContainerTypesAsync / DynamicDataSeeder.SeedWarehouseAsync.
        var addFirst = await client.PostAsJsonAsync("Cart/add-to-cart", new AddToCartCommand
        {
            UserId = userId,
            WarehouseId = 1,
            ContainerType = new ContainerTypeEntity { Id = 1 },
            Quantity = 2
        });
        Assert.Equal(HttpStatusCode.OK, addFirst.StatusCode);

        // Adding the same container type again must merge quantity onto the same line,
        // not create a second cart row (AddToCartCommandHandler.Handle).
        var addSecond = await client.PostAsJsonAsync("Cart/add-to-cart", new AddToCartCommand
        {
            UserId = userId,
            WarehouseId = 1,
            ContainerType = new ContainerTypeEntity { Id = 1 },
            Quantity = 3
        });
        Assert.Equal(HttpStatusCode.OK, addSecond.StatusCode);

        var afterAdd = await client.GetFromJsonAsync<CartResponse>($"Cart/{userId}", JsonOptions);
        var mergedLine = Assert.Single(afterAdd!.CartItems, x => x.ContainerType.Id == 1);
        Assert.Equal(5, mergedLine.Quantity);
        Assert.Equal(CartItemStatus.InCart, mergedLine.CartItemStatus);

        // A distinct container type saved for later must not show up as an in-cart line.
        var save = await client.PostAsJsonAsync("Cart/save-for-later", new SaveForLaterCommand
        {
            UserId = userId,
            WarehouseId = 1,
            ContainerType = new ContainerTypeEntity { Id = 2 },
            Quantity = 1
        });
        Assert.Equal(HttpStatusCode.OK, save.StatusCode);

        // ClearCartCommandHandler only deletes CartItemStatus.InCart rows, so the
        // saved-for-later line must survive the clear.
        var clear = await client.DeleteAsync($"Cart/clear-cart/{userId}");
        Assert.Equal(HttpStatusCode.OK, clear.StatusCode);

        var afterClear = await client.GetFromJsonAsync<CartResponse>($"Cart/{userId}", JsonOptions);
        Assert.DoesNotContain(afterClear!.CartItems, x => x.ContainerType.Id == 1);
        var savedLine = Assert.Single(afterClear.CartItems, x => x.ContainerType.Id == 2);
        Assert.Equal(CartItemStatus.SavedForLater, savedLine.CartItemStatus);
    }

    [Fact]
    public async Task OrderFlow_CreateThenGetById_PersistsCalculatedTotals()
    {
        var (client, userId) = await GetManagerClientAsync();

        // Seeded ContainerType id 2 = price 1m (StaticDataSeeder.SeedContainerTypesAsync).
        var create = await client.PostAsJsonAsync("Order", new CreateOrderCommand
        {
            UserId = userId,
            OrderItems = new List<SharedOrderCommandContainerType>
            {
                new() { ContainerTypeId = 2, WarehouseId = 1, Quantity = 3 }
            }
        });
        Assert.Equal(HttpStatusCode.OK, create.StatusCode);

        var created = await create.Content.ReadFromJsonAsync<CreateOrderCommandDto>(JsonOptions);
        Assert.NotNull(created);
        Assert.True(created!.OrderId > 0);

        var fetched = await client.GetFromJsonAsync<GetOrderByIdQueryDto>($"Order/{created.OrderId}", JsonOptions);
        Assert.NotNull(fetched);
        // OrderConstants.DefaultDiscount is 0m, so Total == Subtotal == price * qty (1 * 3).
        Assert.Equal(3m, fetched!.Subtotal);
        Assert.Equal(3m, fetched.Total);
        var item = Assert.Single(fetched.Items);
        Assert.Equal(2, item.ContainerTypeId);
        Assert.Equal(3, item.Quantity);
    }

    // Local shape for the cart list response: CartItemDto.ContainerType is
    // CartItemContainerTypeDto, which only exposes a (ContainerTypeEntity) constructor and
    // no parameterless one, so System.Text.Json can't deserialize it directly. Reading into
    // this equivalent-shaped POCO sidesteps that instead of reflection-hacking the real DTO.
    private sealed class CartResponse
    {
        public List<CartLine> CartItems { get; set; } = new();
    }

    private sealed class CartLine
    {
        public int Quantity { get; set; }
        public CartItemStatus CartItemStatus { get; set; }
        public ContainerTypeRef ContainerType { get; set; } = null!;
    }

    private sealed class ContainerTypeRef
    {
        public int Id { get; set; }
    }
}
