using Stowaway.Application.Modules.Sales.Order.Shared;
using Stowaway.Domain.Entities.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Sales.Order.Commands.Create
{
    public class CreateOrderCommand : IRequest<CreateOrderCommandDto>
    {
        public required int UserId { get; set; }

        public required List<SharedOrderCommandContainerType> OrderItems { get; set; }
    }

    


}
