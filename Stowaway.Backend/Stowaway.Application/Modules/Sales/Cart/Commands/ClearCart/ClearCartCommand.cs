using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Sales.Cart.Commands.ClearCart
{
    public class ClearCartCommand : IRequest<ClearCartCommandDto>
    {
        public required int UserId { get; set; }
    }
}
