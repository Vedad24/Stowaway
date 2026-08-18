using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.Application.Abstractions;
using Stowaway.Domain.Entities.Sales;
using Stripe;

namespace Market.Application.Modules.Sales.Payment.Commands.Update
{
    public class UpdateStripePaymentCommandHandler(IAppDbContext db) : IRequestHandler<UpdateStripePaymentCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateStripePaymentCommand request, CancellationToken ct)
        {
            switch (request.EventType)
        {
            case "payment_intent.succeeded":
                return await HandlePaymentSucceeded((PaymentIntent)request.EventData, ct);
            case "payment_intent.payment_failed":
                return await HandlePaymentFailed((PaymentIntent)request.EventData, ct);
            default:
                return Unit.Value;
            
        }
        
            
        }

        private async Task<Unit> HandlePaymentSucceeded(PaymentIntent eventData, CancellationToken ct)
        {
            if(!int.TryParse(eventData.Metadata["OrderId"], out var orderId))
                throw new ValidationException("Invalid OrderId in metadata.");
            var order = await db.Orders
                .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken: default);

            if (order is null)
                throw new StowawayNotFoundException($"Order with id {orderId} not found.");

            order.OrderStatusId = OrderStatus.Completed;
            order.PaymentIntentId = eventData.Id;
            order.StripeSessionId = eventData.LatestChargeId;

            await db.SaveChangesAsync(ct);

            return Unit.Value;
        }

        private async Task<Unit> HandlePaymentFailed(PaymentIntent eventData, CancellationToken ct)
        {
            if (!int.TryParse(eventData.Metadata["OrderId"], out var orderId))
                throw new ValidationException("Invalid OrderId in metadata.");
            var order = await db.Orders
                .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken: default);

            if (order is null)
                throw new StowawayNotFoundException($"Order with id {orderId} not found.");

            order.OrderStatusId = OrderStatus.Cancelled;
            order.PaymentIntentId = eventData.Id;

            await db.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
