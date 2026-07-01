using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.Application.Abstractions.Payments;
using Stowaway.Domain.Entities.Sales;

namespace Market.Application.Modules.Sales.Payment.Commands.Create
{
    public class CreatePaymentCommandHandler(IAppDbContext db, IPaymentProvider paymentProvider) : IRequestHandler<CreatePaymentCommand, CreatePaymentResponse>
    {
        public async Task<CreatePaymentResponse> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
             var order = await db.Orders
                .FirstOrDefaultAsync(
                x => x.Id == request.OrderId,
                cancellationToken);

        if (order is null)
            throw new StowawayNotFoundException($"Order with id {request.OrderId} not found.");

        if (order.OrderStatusId == OrderStatus.Completed)
            throw new ValidationException("Order has already been paid.");

        var paymentRequest = new CreatePaymentRequest
        {
            OrderId = order.Id,
            Amount = order.Total,
            Currency = "usd",

            // temporary placeholders
            SuccessUrl = "http://localhost:4200/payment/success",
            CancelUrl = "http://localhost:4200/payment/cancel"
        };

        var paymentResponse =
            await paymentProvider.MakePaymentAsync(paymentRequest);

        return new CreatePaymentResponse(paymentResponse);
        }
    }
}