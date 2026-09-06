using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stowaway.Application.Abstractions.Payments;
using Stowaway.Shared.Options;
using Microsoft.Extensions.Options;
using Stowaway.Domain.Entities.Sales;

namespace Stowaway.Application.Modules.Sales.Payment.Commands.Create
{
    public class CreatePaymentCommandHandler(
        IAppDbContext db,
        IPaymentProvider paymentProvider,
        IOptions<FrontendOptions> frontendOptions,
        IOptions<PaymentOptions> paymentOptions,
        IOptions<StripeOptions> stripeOptions) : IRequestHandler<CreatePaymentCommand, CreatePaymentResponse>
    {
        public async Task<CreatePaymentResponse> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
             var order = await db.Orders
                .FirstOrDefaultAsync(
                x => x.Id == request.OrderId,
                cancellationToken);

        if (order is null)
            throw new StowawayNotFoundException($"Order with id {request.OrderId} not found.");

        if (order.OrderStatusId != OrderStatus.Draft)
            throw new ValidationException($"Order cannot be paid for while in status {order.OrderStatusId}.");

        var frontendBaseUrl = frontendOptions.Value.BaseUrl;

        var paymentRequest = new CreatePaymentRequest
        {
            OrderId = order.Id,
            Amount = order.Total,
            Currency = stripeOptions.Value.Currency,

            SuccessUrl = $"{frontendBaseUrl}/{paymentOptions.Value.SuccessPath}/{order.Id}",
            CancelUrl = $"{frontendBaseUrl}/{paymentOptions.Value.CancelPath}/{order.Id}"
        };

        var paymentResponse =
            await paymentProvider.MakePaymentAsync(paymentRequest);

        return new CreatePaymentResponse(paymentResponse);
        }
    }
}