using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.Application.Abstractions.Payments;

namespace Market.Application.Modules.Sales.Payment.Commands.Create
{
    public class CreatePaymentResponse
    {
        public CreatePaymentResponse(CreatePaymentResult? result)
        {
            CheckoutUrl = result?.CheckoutUrl ?? string.Empty;
            ExternalPaymentId = result?.ExternalPaymentId ?? string.Empty;
        }
        public CreatePaymentResponse()
        {
            
        }
        public string CheckoutUrl { get; set; }
        public string ExternalPaymentId { get; set; }
    }
}