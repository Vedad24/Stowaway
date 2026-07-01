using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Payment.Commands.Update
{
    public sealed class UpdateStripePaymentCommand : IRequest<Unit>
    {
        public string EventType { get; set; } = string.Empty;
        public object EventData { get; set; } = default!;
        public string EventId { get; set; } = string.Empty;
    }
}
