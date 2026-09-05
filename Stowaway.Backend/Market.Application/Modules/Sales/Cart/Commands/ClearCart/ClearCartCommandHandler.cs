using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.Domain.Entities.Sales;
using Market.Shared.Constants;

namespace Market.Application.Modules.Sales.Cart.Commands.ClearCart
{
    public class ClearCartCommandHandler(IAppDbContext db, IAppCurrentUser currentUser) : IRequestHandler<ClearCartCommand, ClearCartCommandDto>
    {
        public async Task<ClearCartCommandDto> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            if(request.UserId != currentUser.UserId && !currentUser.HasPermission(Permissions.CartManageAny))
                throw new StowawayBusinessRuleException(BusinessRuleCodes.CartNotOwner, "Users can only clear their own carts");
            if (await db.Users.AnyAsync(u => u.Id == request.UserId) == false)
            {
                throw new StowawayNotFoundException($"User with id {request.UserId} not found.");
            }

            var cartItems = await db.CartItems
                .Where(x => x.UserId == request.UserId && x.CartItemStatus == CartItemStatus.InCart)
                .ToListAsync(cancellationToken);

            db.CartItems.RemoveRange(cartItems);

            await db.SaveChangesAsync(cancellationToken);
            return new ClearCartCommandDto();
        }
    }
}
