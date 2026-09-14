using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stowaway.Application.Abstractions.Payments;
using Stowaway.Shared.Options;
using Microsoft.Extensions.Options;
using Stowaway.Domain.Entities.Sales;
using Stowaway.Shared.Constants;

namespace Stowaway.Application.Modules.Sales.Payment.Commands.Create
{
    public class CreatePaymentCommandHandler(
        IAppDbContext db,
        IPaymentProvider paymentProvider,
        IOptions<FrontendOptions> frontendOptions,
        IOptions<PaymentOptions> paymentOptions,
        IOptions<StripeOptions> stripeOptions,
        IAppCurrentUser currentUser) : IRequestHandler<CreatePaymentCommand, CreatePaymentResponse>
    {
        public async Task<CreatePaymentResponse> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            
            var order = await db.Orders
            .FirstOrDefaultAsync(
            x => x.Id == request.OrderId,
            cancellationToken);

            if (order is null)
                throw new StowawayNotFoundException($"Order with id {request.OrderId} not found.");

            if(currentUser.UserId != order.UserId && !currentUser.HasPermission(Permissions.OrderCreateAny))
                throw new StowawayBusinessRuleException(BusinessRuleCodes.OrderNotOwner, "You can pay only for your own orders");

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