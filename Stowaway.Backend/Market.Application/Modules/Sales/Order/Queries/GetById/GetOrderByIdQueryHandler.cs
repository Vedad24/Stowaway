using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Sales.Order.Queries.GetById
{
    public class GetOrderByIdQueryHandler(IAppDbContext db, IAppCurrentUser currentUser) : IRequestHandler<GetOrderByIdQuery, GetOrderByIdQueryDto>
    {
        public async Task<GetOrderByIdQueryDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            //Get order with User
            var order = await db.Orders.Include(o => o.User).FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
            if(order is null || (!currentUser.IsAdmin && order.UserId != currentUser.UserId))
            {
                throw new StowawayNotFoundException($"Order with ID {request.Id} doesn't exist");
            }

            //Get order items of order
            var orderItems = await db.OrderItems.Include(oi => oi.ContainerType).Where(oi => oi.OrderId == request.Id).Select(oi => new GetOrderByIdQueryDtoOrderItem
            {
                ContainerType = oi.ContainerType.ToString(),
                Quantity = oi.Quantity,
                Total = oi.Total,
                UnitPrice = oi.UnitPrice
            })
            .ToListAsync(cancellationToken);

            //Map to result
            var result = new GetOrderByIdQueryDto
            {
                OrderDate = order.OrderDate,
                Items = orderItems,
                OrderStatus = order.OrderStatusId.ToString(),
                Subtotal = order.Subtotal,
                Total = order.Total,
                User = new GetOrderByIdQueryDtoUser
                {
                    Email = order.User.Email,
                    Name = $"{order.User.FirstName} {order.User.LastName}"
                }
            };
            return result;
        }
    }
}
