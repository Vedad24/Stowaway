using Stowaway.API.Authorization;
using Stowaway.Application.Modules.Sales.Cart.Commands.AddToCart;
using Stowaway.Application.Modules.Sales.Cart.Commands.ClearCart;
using Stowaway.Application.Modules.Sales.Cart.Commands.SaveForLater;
using Stowaway.Application.Modules.Sales.Cart.Queries.List;
using Stowaway.Shared.Constants;

namespace Stowaway.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class CartController(ISender sender) : ControllerBase
    {
        [HttpGet("{userId:int}")]
        [HasPermission(Permissions.CartManage)]
        public async Task<ListCartItemQueryDto> ListCartItems(int userId, CancellationToken ct)
        {
            return await sender.Send(new ListCartItemsQuery { UserId = userId }, ct);
        }

        [HttpPost("add-to-cart")]
        [HasPermission(Permissions.CartManage)]
        public async Task<AddToCartCommandDto> AddToCart([FromBody] AddToCartCommand command, CancellationToken ct)
        {
            return await sender.Send(command, ct);
        }

        [HttpPost("save-for-later")]
        [HasPermission(Permissions.CartManage)]
        public async Task<SaveForLaterCommandDto> SaveForLater([FromBody] SaveForLaterCommand command, CancellationToken ct)
        {
            return await sender.Send(command, ct);
        }

        [HttpDelete("clear-cart/{userId:int}")]
        [HasPermission(Permissions.CartManage)]
        public async Task<ClearCartCommandDto> ClearCart(int userId, CancellationToken ct)
        {
            return await sender.Send(new ClearCartCommand { UserId = userId }, ct);
        }
    }
}
