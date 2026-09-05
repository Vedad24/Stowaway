using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.Domain.Entities.Sales;
using Market.Shared.Constants;

namespace Market.Application.Modules.Sales.Cart.Commands.AddToCart
{
    public class AddToCartCommandHandler(IAppDbContext db, IAppCurrentUser currentUser) : IRequestHandler<AddToCartCommand, AddToCartCommandDto>
    {
        public async Task<AddToCartCommandDto> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId != currentUser.UserId && !currentUser.HasPermission(Permissions.CartManageAny))
                throw new StowawayBusinessRuleException(BusinessRuleCodes.CartNotOwner, "Users can only add to their own carts");
            if (await db.Warehouses.AnyAsync(w => w.Id == request.WarehouseId) == false)
            {
                throw new StowawayNotFoundException($"Warehouse with id {request.WarehouseId} not found.");
            }
            if(await db.ContainerTypes.AnyAsync(ct => ct.Id == request.ContainerType.Id) == false)
            {
                throw new StowawayNotFoundException($"Container type with id: {request.ContainerType.Id} not found.");
            }
            //find item
            var existingItem = await db.CartItems.Where(x => x.UserId == request.UserId && x.ContainerType.Id == request.ContainerType.Id).FirstOrDefaultAsync(cancellationToken);
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
                    CartItemStatus = CartItemStatus.InCart
                };
                db.ContainerTypes.Attach(CartItem.ContainerType);
                db.CartItems.Add(CartItem);

            }
            //Find if in saveForLater
            else if(existingItem.CartItemStatus == CartItemStatus.SavedForLater)
            {
                existingItem.CartItemStatus = CartItemStatus.InCart;
            }
            else
            {
                //if already in cart update quantity
                existingItem.Quantity += request.Quantity;
            } 
            await db.SaveChangesAsync(cancellationToken);
            return new AddToCartCommandDto();
        }
    }
}