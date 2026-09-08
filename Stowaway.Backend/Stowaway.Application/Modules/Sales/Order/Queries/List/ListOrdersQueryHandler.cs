using Stowaway.Application.Common;
using Stowaway.Domain.Entities.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Sales.Order.Queries.List
{
    public class ListOrdersQueryHandler(IAppDbContext db, IAppCurrentUser currentUser) : IRequestHandler<ListOrdersQuery, PageResult<ListOrdersQueryDto>>
    {
        public async Task<PageResult<ListOrdersQueryDto>> Handle(ListOrdersQuery request, CancellationToken cancellationToken)
        {
            var allOrders = db.Orders.Include(o => o.User).AsNoTracking();
            if (!currentUser.IsAdmin)
            {
                allOrders = allOrders.Where(o => o.UserId == currentUser.UserId);
            }
            if (!string.IsNullOrEmpty(request.SearchByUserEmail))
            {
                allOrders = allOrders.Where(o => o.User.Email.Contains(request.SearchByUserEmail));
            }
            if (!string.IsNullOrEmpty(request.SearchByUserName))
            {
                allOrders = allOrders.Where(o => (o.User.FirstName + " " + o.User.LastName).Contains(request.SearchByUserName));
            }
            if(!string.IsNullOrEmpty(request.SearchByWarehouseName))
            {
                allOrders = allOrders.Where(o => o.OrderItems!.Any(oi => oi.Warehouse!.Name.Contains(request.SearchByWarehouseName)));
            }
            if(request.SearchByStatus is not null)
            {
                allOrders = allOrders.Where(o => o.OrderStatusId == request.SearchByStatus);
            }
            if(request.CreateTimeMin is not null)
            {
                allOrders = allOrders.Where(o => request.CreateTimeMin < o.OrderDate);
            }
            if(request.CreateTimeMax is not null)
            {
                allOrders = allOrders.Where(o => o.OrderDate < request.CreateTimeMax);
            }
            var resultQuery = allOrders.Select(o => new ListOrdersQueryDto
            {
                Id = o.Id,
                OrderStatus = (o.OrderStatus == null) ? "Draft" : Enum.GetName(o.OrderStatusId),
                User = new ListOrdersQueryDtoUser { Email = o.User.Email, Name = $"{o.User.FirstName} {o.User.LastName}" },
                OrderDate = o.OrderDate,
                Subtotal = o.Subtotal,
                Total = o.Total
            });
            return await PageResult<ListOrdersQueryDto>.FromQueryableAsync(resultQuery, request.Paging, cancellationToken);
        }

        
    }
}
