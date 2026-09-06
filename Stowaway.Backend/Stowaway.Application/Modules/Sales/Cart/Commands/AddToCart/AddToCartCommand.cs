using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stowaway.Domain.Entities.Storage;

namespace Stowaway.Application.Modules.Sales.Cart.Commands.AddToCart
{
    public class AddToCartCommand : IRequest<AddToCartCommandDto>
    {
        public required int UserId { get; set; }
        public required int WarehouseId { get; set; }
        public required ContainerTypeEntity ContainerType { get; set; }
        public required int Quantity { get; set; }
    }
}