using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Cart.Queries.List
{
    public class ListCartItemsQueryHandler(IAppDbContext db) : IRequestHandler<ListCartItemsQuery, ListCartItemQueryDto>
    {
        public async Task<ListCartItemQueryDto> Handle(ListCartItemsQuery request, CancellationToken cancellationToken)
        {
            if(db.Users.Any(u => u.Id == request.UserId) == false)
            {
                throw new StowawayNotFoundException($"User with id {request.UserId} not found.");
            }
            ListCartItemQueryDto result = new()
            {
                CartItems = await db.CartItems.Where(x => x.UserId == request.UserId)
                .Select(x => new CartItemDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    ContainerType = x.ContainerType,
                    WarehouseId = x.WarehouseId,
                    Quantity = x.Quantity,
                    CartItemStatus = x.CartItemStatus
                }).ToListAsync()
            };
            return result;
        }
    }
}