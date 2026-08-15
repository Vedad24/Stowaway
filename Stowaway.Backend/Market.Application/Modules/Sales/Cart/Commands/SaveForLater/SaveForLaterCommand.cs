using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stowaway.Domain.Entities.Storage;

namespace Market.Application.Modules.Sales.Cart.Commands.SaveForLater
{
    public class SaveForLaterCommand : IRequest<SaveForLaterCommandDto>
    {
        public required int UserId { get; set; }
        public required int WarehouseId { get; set; }
        public required ContainerTypeEntity ContainerType { get; set; }
        public required int Quantity { get; set; }
    }
}