using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stowaway.Domain.Entities.Storage;

namespace Market.Application.Modules.Sales.Cart.Commands.AddToCart
{
    public class AddToCartCommand : IRequest<AddToCartCommandDto>
    {
        public required int UserId { get; set; }
        public required ContainerTypeEntity ContainerType { get; set; }
        public required int Quantity { get; set; }
    }
}