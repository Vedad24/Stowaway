using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.Shared.Constants;

namespace Market.Application.Modules.Sales.Cart.Queries.List
{
    public class ListCartItemsQueryHandler(IAppDbContext db, IAppCurrentUser currentUser) : IRequestHandler<ListCartItemsQuery, ListCartItemQueryDto>
    {
        public async Task<ListCartItemQueryDto> Handle(ListCartItemsQuery request, CancellationToken cancellationToken)
        {
            if (request.UserId != currentUser.UserId && !currentUser.HasPermission(Permissions.CartManageAny))
                throw new StowawayBusinessRuleException("P-C-S", "Users can only view their own carts");
            if(db.Users.Any(u => u.Id == request.UserId) == false)
            {
                throw new StowawayNotFoundException($"User with id {request.UserId} not found.");
            }
            var cartItems = await db.CartItems.Include(ci => ci.ContainerType).AsNoTracking().Where(x => x.UserId == request.UserId).ToListAsync(cancellationToken);

            var warehouseIds = cartItems.Select(x => x.WarehouseId).Distinct().ToList();
            var warehouseNameById = await db.Warehouses
                .Where(w => warehouseIds.Contains(w.Id))
                .ToDictionaryAsync(w => w.Id, w => w.Name, cancellationToken);

            ListCartItemQueryDto result = new()
            {
                CartItems = cartItems.Select(x => new CartItemDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    ContainerType = new(x.ContainerType),
                    WarehouseId = x.WarehouseId,
                    WarehouseName = warehouseNameById.GetValueOrDefault(x.WarehouseId, string.Empty),
                    Quantity = x.Quantity,
                    CartItemStatus = x.CartItemStatus
                }).ToList()
            };
            return result;
        }
    }
}