using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.Domain.Entities.Sales;
using Market.Shared.Constants;

namespace Market.Application.Modules.Sales.Cart.Commands.SaveForLater
{
    public class SaveForLaterCommandHandler(IAppDbContext db, IAppCurrentUser currentUser) : IRequestHandler<SaveForLaterCommand, SaveForLaterCommandDto>
    {
        public async Task<SaveForLaterCommandDto> Handle(SaveForLaterCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId != currentUser.UserId && !currentUser.HasPermission(Permissions.CartManageAny))
                throw new StowawayBusinessRuleException("P-C-S", "Users can only save items to their own carts");
            if (await db.Warehouses.AnyAsync(w => w.Id == request.WarehouseId) == false)
            {
                throw new StowawayNotFoundException($"Warehouse with id {request.WarehouseId} not found.");
            }

            //find item
            var existingItem = await db.CartItems.Where(x => x.UserId == request.UserId && x.ContainerType.Id == request.ContainerType.Id).FirstOrDefaultAsync();
            //if not found add to cart 
            if(existingItem == null)
            {
                //throw new NotImplementedException();
                var CartItem = new CartItemEntity
                {
                    UserId = request.UserId,
                    ContainerType = request.ContainerType,
                    WarehouseId = request.WarehouseId,
                    Quantity = request.Quantity,
                    CartItemStatus = CartItemStatus.SavedForLater
                };
                db.ContainerTypes.Attach(CartItem.ContainerType);
                db.CartItems.Add(CartItem);

            }
            //Find if in inCart
            else if(existingItem.CartItemStatus == CartItemStatus.InCart)
            {
                existingItem.CartItemStatus = CartItemStatus.SavedForLater;
            }
            else
            {
                //if already savedForLater update quantity
                existingItem.Quantity += request.Quantity;
            } 
            await db.SaveChangesAsync(cancellationToken);
            return new SaveForLaterCommandDto();
        }
    }
}