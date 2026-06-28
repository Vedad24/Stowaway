using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Payment.Commands.Create
{
    public sealed record CreatePaymentCommand(int OrderId) : IRequest<CreatePaymentResponse>
    {
        
    }
}