using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Market.API.Authorization;
using Market.Application.Modules.Sales.Payment.Commands.Create;
using Market.Application.Modules.Sales.Payment.Commands.Update;
using Market.Shared.Constants;
using Market.Shared.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;

namespace Market.API.Controllers
{
    [Route("[controller]")]
    public class StripePaymentController : ControllerBase
    {
        private readonly ILogger<StripePaymentController> logger;
        private readonly ISender sender;
        private readonly IOptions<StripeOptions> stripeOptions;

        public StripePaymentController(ILogger<StripePaymentController> logger, ISender sender, IOptions<StripeOptions> stripeOptions)
        {
            this.sender = sender;
            this.logger = logger;
            this.stripeOptions = stripeOptions;
        }

        [HttpPost("pay")]
        [HasPermission(Permissions.OrderCreate)]
        public async Task<IActionResult> MakePayment(
        [FromBody]CreatePaymentCommand command,
        CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            return Ok(result);
        }

        
        //stripe listen --forward-to localhost:5177/StripePayment/payment-webhook
        //Change wehbook secret in appsettings.Development.json
        [HttpPost("payment-webhook")]
        [AllowAnonymous]
        public async Task<ActionResult> PaymentWebhook()
        {
             var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                stripeOptions.Value.WebhookSecret
            );

            // Send to MediatR
            var command = new UpdateStripePaymentCommand
            {
                EventType = stripeEvent.Type,
                EventData = stripeEvent.Data.Object,
                EventId = stripeEvent.Id
            };

            await sender.Send(command);
            return Ok();
        }
        catch (StripeException)
        {
            return BadRequest();
        }
        }
    }
}