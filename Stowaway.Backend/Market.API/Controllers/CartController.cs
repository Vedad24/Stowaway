using Market.Application.Modules.Sales.Cart.Commands.AddToCart;
using Market.Application.Modules.Sales.Cart.Commands.SaveForLater;
using Market.Application.Modules.Sales.Cart.Queries.List;

namespace Stowaway.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous] // delete later
    public class CartController(ISender sender) : ControllerBase
    {
        [HttpGet("{userId:int}")]
        public async Task<ListCartItemQueryDto> ListCartItems(int userId, CancellationToken ct)
        {
            return await sender.Send(new ListCartItemsQuery { UserId = userId }, ct);
        }

        [HttpPost("add-to-cart")]
        public async Task<AddToCartCommandDto> AddToCart([FromBody] AddToCartCommand command, CancellationToken ct)
        {
            return await sender.Send(command, ct);
        }

        [HttpPost("save-for-later")]
        public async Task<SaveForLaterCommandDto> SaveForLater([FromBody] SaveForLaterCommand command, CancellationToken ct)
        {
            return await sender.Send(command, ct);
        }
    }
}
