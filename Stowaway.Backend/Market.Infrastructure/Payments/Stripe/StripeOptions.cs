using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Market.Infrastructure.Payments.Stripe
{
    public class StripeOptions
    {
        public string ApiKey { get; set; } = null!;
        public string WebhookSecret { get; set; } = null!;
    }
}