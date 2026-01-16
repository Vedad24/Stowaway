using Stowaway.Application.Modules.Sales.Order.Commands.Create;
using Stowaway.Application.Modules.Sales.Order.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Sales.Order.Commands.Update
{
    public class UpdateOrderCommand : IRequest<bool>
    {
        public int Id { get; set; }
        
        //this is basically a new list of containers, you can replace the old one 
        public List<SharedOrderCommandContainerType> allContainerTypes { get; set; }
    }
}
