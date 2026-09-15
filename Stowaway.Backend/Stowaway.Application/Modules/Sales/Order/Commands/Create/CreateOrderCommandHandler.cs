using Stowaway.Shared.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Stowaway.Application.Modules.Sales.Order.Shared;
using Stowaway.Domain.Entities.Sales;
using Stowaway.Domain.Entities.Storage;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Sales.Order.Commands.Create
{
    public class CreateOrderCommandHandler(IAppDbContext db, IAppCurrentUser currentUser) : IRequestHandler<CreateOrderCommand, CreateOrderCommandDto>
    {
        public async Task<CreateOrderCommandDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId != currentUser.UserId && !currentUser.HasPermission(Permissions.OrderCreateAny))
                throw new StowawayBusinessRuleException(BusinessRuleCodes.OrderNotOwner, "Users can only place orders for themselves");

            #region MakeOrder
            var order = new OrderEntity
            {
                //Id = 0, //auto
                UserId = request.UserId,

                OrderStatusId = OrderStatus.Draft,
                //OrderStatus = null //auto

                OrderDate = DateTime.UtcNow,

                Total = 0, //calculate
                Subtotal = 0, //calculate

            };
            db.Orders.Add(order);
            #endregion

            #region DeduplicateOrderItems
            List<SharedOrderCommandContainerType> dedupedOrderItems = request.OrderItems
                .GroupBy(item => (item.ContainerTypeId, item.WarehouseId))
                .Select(group => new SharedOrderCommandContainerType
                {
                    ContainerTypeId = group.Key.ContainerTypeId,
                    WarehouseId = group.Key.WarehouseId,
                    Quantity = group.Sum(item => item.Quantity),
                })
                .ToList();
            #endregion

            #region getOrderItemMap
            List<int> orderedContainerTypeIds = dedupedOrderItems.Select(ct => ct.ContainerTypeId).ToList();

            List<ContainerTypeEntity> orderedContainerTypes = await db.ContainerTypes
                .Where(ct => orderedContainerTypeIds.Contains(ct.Id))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            Dictionary<int, ContainerTypeEntity> dictionaryIdsContainerTypes = orderedContainerTypes.ToDictionary(oct => oct.Id);
            #endregion

            #region PrepareOrderItems
            decimal discount = OrderConstants.DefaultDiscount;

            foreach (var item in dedupedOrderItems)
            {
                var containerType = dictionaryIdsContainerTypes.GetValueOrDefault(item.ContainerTypeId);
                if (containerType is null)
                    throw new StowawayNotFoundException("Container type does not exist ->" + item.ContainerTypeId);
                db.ContainerTypes.Attach(containerType);

                decimal subtotal = RoundMoney(containerType.Price * item.Quantity);
                decimal total = RoundMoney(subtotal * (1 - discount));

                var newOrderItemEntity = new OrderItemEntity
                {
                    //Id = 0, //auto

                    //OrderId = 0, // auto
                    Order = order,

                    //ContainerTypeId = item.ContainerTypeId, // auto
                    ContainerType = containerType,

                    WarehouseId = item.WarehouseId,

                    UnitPrice = containerType.Price,
                    Quantity = item.Quantity,

                    Discount = discount,
                    Subtotal = subtotal,
                    Total = total
                };
                db.OrderItems.Add(newOrderItemEntity);
                order.Total += RoundMoney(newOrderItemEntity.Total);
                order.Subtotal += RoundMoney(newOrderItemEntity.Subtotal);
            }
            #endregion

            await db.SaveChangesAsync(cancellationToken);

            return new() { OrderId = order.Id };
        }
        private static decimal RoundMoney(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }
    }

}
