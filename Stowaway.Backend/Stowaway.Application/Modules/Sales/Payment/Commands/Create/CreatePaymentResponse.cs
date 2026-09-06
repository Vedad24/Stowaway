using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stowaway.Application.Abstractions.Payments;

namespace Stowaway.Application.Modules.Sales.Payment.Commands.Create
{
    public class CreatePaymentResponse
    {
        public string CheckoutUrl { get; set; }
        public string ExternalPaymentId { get; set; }
        public string Status { get; set; }

        public CreatePaymentResponse(CreatePaymentResult? result)
        {
            CheckoutUrl = result?.CheckoutUrl ?? string.Empty;
            ExternalPaymentId = result?.ExternalPaymentId ?? string.Empty;
            Status = result?.Status ?? string.Empty;
        }
        
    }
}