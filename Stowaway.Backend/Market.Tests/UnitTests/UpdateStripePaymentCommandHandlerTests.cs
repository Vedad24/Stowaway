using Market.Application.Modules.Sales.Payment.Commands.Update;
using Market.Tests.Helpers;
using Stowaway.Domain.Entities.Sales;
using Stripe;

namespace Market.Tests.UnitTests;

public class UpdateStripePaymentCommandHandlerTests
{
    [Fact]
    public async Task Handle_CompletesOrder_OnPaymentSucceeded()
    {
        await using var db = TestDbContext.Create();
        db.Orders.Add(new OrderEntity { Id = 1, UserId = 1, Subtotal = 100m, Total = 100m, OrderDate = DateTime.UtcNow, OrderStatusId = OrderStatus.Draft });
        await db.SaveChangesAsync(CancellationToken.None);
        db.ChangeTracker.Clear();

        var handler = new UpdateStripePaymentCommandHandler(db);
        var eventData = new PaymentIntent { Id = "pi_123", LatestChargeId = "ch_123", Metadata = new Dictionary<string, string> { { "OrderId", "1" } } };

        await handler.Handle(new UpdateStripePaymentCommand { EventType = "payment_intent.succeeded", EventData = eventData, EventId = "evt_1" }, CancellationToken.None);

        var order = await db.Orders.FindAsync(1);
        Assert.Equal(OrderStatus.Completed, order!.OrderStatusId);
        Assert.Equal("pi_123", order.PaymentIntentId);
        Assert.Equal("ch_123", order.StripeSessionId);
        Assert.True(await db.ProcessedStripeEvents.AnyAsync(x => x.EventId == "evt_1"));
    }

    [Fact]
    public async Task Handle_CancelsOrder_OnPaymentFailed()
    {
        await using var db = TestDbContext.Create();
        db.Orders.Add(new OrderEntity { Id = 1, UserId = 1, Subtotal = 100m, Total = 100m, OrderDate = DateTime.UtcNow, OrderStatusId = OrderStatus.Draft });
        await db.SaveChangesAsync(CancellationToken.None);
        db.ChangeTracker.Clear();

        var handler = new UpdateStripePaymentCommandHandler(db);
        var eventData = new PaymentIntent { Id = "pi_456", Metadata = new Dictionary<string, string> { { "OrderId", "1" } } };

        await handler.Handle(new UpdateStripePaymentCommand { EventType = "payment_intent.payment_failed", EventData = eventData, EventId = "evt_2" }, CancellationToken.None);

        var order = await db.Orders.FindAsync(1);
        Assert.Equal(OrderStatus.Cancelled, order!.OrderStatusId);
        Assert.True(await db.ProcessedStripeEvents.AnyAsync(x => x.EventId == "evt_2"));
    }

    [Fact]
    public async Task Handle_IsNoOp_OnDuplicateEventId()
    {
        await using var db = TestDbContext.Create();
        db.Orders.Add(new OrderEntity { Id = 1, UserId = 1, Subtotal = 100m, Total = 100m, OrderDate = DateTime.UtcNow, OrderStatusId = OrderStatus.Draft });
        db.ProcessedStripeEvents.Add(new ProcessedStripeEventEntity { EventId = "evt_dup", ProcessedAtUtc = DateTime.UtcNow });
        await db.SaveChangesAsync(CancellationToken.None);
        db.ChangeTracker.Clear();

        var handler = new UpdateStripePaymentCommandHandler(db);
        var eventData = new PaymentIntent { Id = "pi_789", Metadata = new Dictionary<string, string> { { "OrderId", "1" } } };

        await handler.Handle(new UpdateStripePaymentCommand { EventType = "payment_intent.succeeded", EventData = eventData, EventId = "evt_dup" }, CancellationToken.None);

        var order = await db.Orders.FindAsync(1);
        Assert.Equal(OrderStatus.Draft, order!.OrderStatusId);
        Assert.Null(order.PaymentIntentId);
    }
}
