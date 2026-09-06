using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Sales.Payment.Commands.Create
{
    public sealed record CreatePaymentCommand : IRequest<CreatePaymentResponse>
    {
        public int OrderId {get; set;}
    }
}