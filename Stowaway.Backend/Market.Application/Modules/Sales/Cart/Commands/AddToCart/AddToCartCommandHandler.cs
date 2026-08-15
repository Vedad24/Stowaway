using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.Domain.Entities.Sales;

namespace Market.Application.Modules.Sales.Cart.Commands.AddToCart
{
    public class AddToCartCommandHandler(IAppDbContext db) : IRequestHandler<AddToCartCommand, AddToCartCommandDto>
    {
        public async Task<AddToCartCommandDto> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
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