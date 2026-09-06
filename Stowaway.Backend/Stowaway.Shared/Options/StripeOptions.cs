using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Stowaway.Shared.Options
{
    public class StripeOptions
    {
        public const string SectionName = "Stripe";

        [Required]
        public string ApiKey { get; set; } = null!;

        [Required]
        public string WebhookSecret { get; set; } = null!;

        [Required]
        public string Currency { get; set; } = null!;
    }
}