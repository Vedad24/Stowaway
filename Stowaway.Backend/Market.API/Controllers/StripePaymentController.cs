using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Market.Application.Modules.Sales.Payment.Commands.Create;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Market.API.Controllers
{
    [Route("[controller]")]
    public class StripePaymentController : ControllerBase
    {
        private readonly ILogger<StripePaymentController> logger;
        private readonly ISender sender;

        public StripePaymentController(ILogger<StripePaymentController> logger, ISender sender)
        {
            this.sender = sender;
            this.logger = logger;
        }

        [HttpPost("test")]
        public async Task<IActionResult> MakePayment(
        CreatePaymentCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return Ok(result);
    }
    }
}