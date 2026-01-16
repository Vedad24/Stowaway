using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
    public class CreateOrderCommandHandler(IAppDbContext db) : IRequestHandler<CreateOrderCommand, CreateOrderCommandDto>
    {
        public async Task<CreateOrderCommandDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {

            #region MakeOrder
            var order = new OrderEntity
            {
                //Id = 0, //auto
                UserId = request.UserId,

                OrderStatusId = OrderStatus.Draft,
                //OrderStatus = null //auto

                OrderDate = DateTime.Now,

                Total = 0, //calculate
                Subtotal = 0, //calculate

            };
            db.Orders.Add(order);
            #endregion

            #region getOrderItemMap
            List<int> orderedContainerTypeIds = request.OrderItems.Select(ct => ct.ContainerTypeId).ToList();

            List<ContainerTypeEntity> orderedContainerTypes = await db.ContainerTypes
                .Where(ct => orderedContainerTypeIds.Contains(ct.Id))
                .AsNoTracking()
                .ToListAsync();

            Dictionary<int, ContainerTypeEntity> dictionaryIdsContainerTypes = orderedContainerTypes.ToDictionary(oct => oct.Id);
            #endregion

            #region PrepareOrderItems
            //demo
            decimal discount = 0.05m; //update somewhere pls

            foreach (var item in request.OrderItems)
            {
                var containerType = dictionaryIdsContainerTypes.GetValueOrDefault(item.ContainerTypeId);
                if (containerType is null)
                    throw new StowawayNotFoundException("Ne postoji container type ->" + item.ContainerTypeId);
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

                    UnitPrice = containerType.Price,
                    Quantity = item.Quantity,

                    Discount = discount, //maybe add later
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
