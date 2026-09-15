using Microsoft.Extensions.Options;
using Stowaway.Application.Abstractions.Payments;
using Stowaway.Application.Common.Exceptions;
using Stowaway.Application.Modules.Sales.Payment.Commands.Create;
using Stowaway.Domain.Entities.Sales;
using Stowaway.Shared.Constants;
using Stowaway.Shared.Options;
using Stowaway.Tests.Helpers;

namespace Stowaway.Tests.UnitTests;

public class CreatePaymentCommandHandlerTests
{
    private static IOptions<FrontendOptions> FrontendOptions() =>
        Options.Create(new FrontendOptions { BaseUrl = "https://frontend.test" });

    private static IOptions<PaymentOptions> PaymentOptions() =>
        Options.Create(new PaymentOptions { SuccessPath = "success", CancelPath = "cancel" });

    private static IOptions<StripeOptions> StripeOptions() =>
        Options.Create(new StripeOptions { ApiKey = "k", WebhookSecret = "s", Currency = "usd" });

    private static CreatePaymentCommandHandler CreateHandler(TestDbContext db, FakeCurrentUser user) =>
        new(db, new FakePaymentProvider(), FrontendOptions(), PaymentOptions(), StripeOptions(), user);

    private static async Task<int> SeedDraftOrder(TestDbContext db, int ownerUserId)
    {
        var order = new OrderEntity
        {
            UserId = ownerUserId,
            Subtotal = 100m,
            Total = 100m,
            OrderDate = DateTime.UtcNow,
            OrderStatusId = OrderStatus.Draft
        };
        db.Orders.Add(order);
        await db.SaveChangesAsync(CancellationToken.None);
        return order.Id;
    }

    [Fact]
    public async Task Handle_Succeeds_WhenManagerPaysForOwnOrder()
    {
        await using var db = TestDbContext.Create();
        var orderId = await SeedDraftOrder(db, ownerUserId: 1);

        var handler = CreateHandler(db, new FakeCurrentUser { UserId = 1, IsManager = true });

        var result = await handler.Handle(new CreatePaymentCommand { OrderId = orderId }, CancellationToken.None);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_ThrowsBusinessRuleException_WhenManagerPaysForAnotherUsersOrder()
    {
        await using var db = TestDbContext.Create();
        var orderId = await SeedDraftOrder(db, ownerUserId: 1);

        var handler = CreateHandler(db, new FakeCurrentUser { UserId = 2, IsManager = true });

        await Assert.ThrowsAsync<StowawayBusinessRuleException>(() =>
            handler.Handle(new CreatePaymentCommand { OrderId = orderId }, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Succeeds_WhenAdminPaysForOwnOrder()
    {
        await using var db = TestDbContext.Create();
        var orderId = await SeedDraftOrder(db, ownerUserId: 1);

        var handler = CreateHandler(db, new FakeCurrentUser { UserId = 1, IsAdmin = true, Permissions = [Permissions.OrderCreateAny] });

        var result = await handler.Handle(new CreatePaymentCommand { OrderId = orderId }, CancellationToken.None);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_Succeeds_WhenAdminPaysForAnotherUsersOrder()
    {
        await using var db = TestDbContext.Create();
        var orderId = await SeedDraftOrder(db, ownerUserId: 1);

        var handler = CreateHandler(db, new FakeCurrentUser { UserId = 2, IsAdmin = true, Permissions = [Permissions.OrderCreateAny] });

        var result = await handler.Handle(new CreatePaymentCommand { OrderId = orderId }, CancellationToken.None);

        Assert.NotNull(result);
    }

    private sealed class FakePaymentProvider : IPaymentProvider
    {
        public Task<CreatePaymentResult> MakePaymentAsync(CreatePaymentRequest request) =>
            Task.FromResult(new CreatePaymentResult
            {
                CheckoutUrl = "https://checkout.test",
                ExternalPaymentId = "ext_123",
                Status = "created"
            });
    }
}
