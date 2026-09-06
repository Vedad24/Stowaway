using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stowaway.Application.Abstractions.Payments;
using Stowaway.Shared.Options;
using Stripe;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using Stowaway.Application.Modules.Sales.Payment.Commands.Create;

namespace Stowaway.Infrastructure.Payments.Stripe
{
    public class StripePaymentProvider : IPaymentProvider
    {
        
        public StripePaymentProvider(IOptions<StripeOptions> options)
        {
            StripeConfiguration.ApiKey = options.Value.ApiKey;    
        }

        public async Task<CreatePaymentResult> MakePaymentAsync(CreatePaymentRequest request)
        {
            var options = new SessionCreateOptions
            {
                ClientReferenceId = request.OrderId.ToString(),
                
                Mode = "payment",

                SuccessUrl = request.SuccessUrl,

                CancelUrl = request.CancelUrl,

                LineItems = new List<SessionLineItemOptions>
                {
                    new()
                    {
                        Quantity = 1,

                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = request.Currency,

                            UnitAmountDecimal = request.Amount * 100,

                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"Order #{request.OrderId}"
                            }
                        }
                    }
                },

                Metadata = new Dictionary<string, string>
                {
                    { "OrderId", request.OrderId.ToString() }
                },
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    Metadata = new Dictionary<string, string>
                    {
                        { "OrderId", request.OrderId.ToString() }
                    }
                }
            };

            var requestOptions = new RequestOptions
            {
                IdempotencyKey = BuildCheckoutIdempotencyKey(request.OrderId)
            };

            var service = new SessionService();

            var session = await service.CreateAsync(options, requestOptions);

            return new CreatePaymentResult
            {
                CheckoutUrl = session.Url,
                ExternalPaymentId = session.PaymentIntentId,
                Status = session.PaymentStatus
            };
        }

        public static string BuildCheckoutIdempotencyKey(int orderId) => $"order-{orderId}-checkout";
    }
}