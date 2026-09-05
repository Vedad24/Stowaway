using MediatR;
using Market.Shared.Constants;
using Stowaway.Domain.Entities.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Sales.Order.Commands.Update
{
    public class UpdateOrderCommandHandler(IAppDbContext db) : IRequestHandler<UpdateOrderCommand, bool>
    {
        public async Task<bool> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            //Trebalo bi biti sporo, ali bi trebalo da radi...pa sad...
            var order = await db.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == request.Id);
            if (order == null)
            {
                throw new StowawayNotFoundException($"Order with id {request.Id} doesn't exist");
            }
            if (order.OrderStatusId is OrderStatus.Completed or OrderStatus.Cancelled or OrderStatus.Refunded)
            {
                throw new StowawayBusinessRuleException("order.locked", $"Cannot modify an order once it is {order.OrderStatusId}.");
            }
            #region Convert request container types
            var lstTypes = request.allContainerTypes.Select(ct => ct.ContainerTypeId).ToList();
            #endregion
            #region Get containerTypes
            var typeDictionary = db.ContainerTypes.Where(ct => lstTypes.Contains(ct.Id)).ToDictionary(x => (int)x.Id);
            #endregion
            
            if (request.allContainerTypes.Count <= 0)
                throw new StowawayBusinessRuleException(BusinessRuleCodes.OrderEmpty, "Orders must have items in them");
            order.Subtotal = 0;
            order.Total = 0;
            var newItems = request.allContainerTypes.Select(item =>
            {

                Domain.Entities.Storage.ContainerTypeEntity type = typeDictionary[item.ContainerTypeId];
                decimal discount = OrderConstants.DefaultDiscount;
                decimal sub = item.Quantity * type.Price;
                decimal total = sub * (1-discount);
                order.Subtotal += sub;
                order.Total += total;

                return new OrderItemEntity
                {
                    Order = order,
                    ContainerType = type,
                    Discount = discount,
                    Quantity = item.Quantity,
                    WarehouseId = item.WarehouseId,
                    Subtotal = sub,
                    Total = total,
                    UnitPrice = type.Price,
                };

            }).ToList();
            
            order.OrderItems = newItems;
            await db.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
