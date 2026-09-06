using Stowaway.API.Authorization;
using Stowaway.Shared.Constants;
using Stowaway.Application.Modules.Sales.Order.Commands.Create;
using Stowaway.Application.Modules.Sales.Order.Commands.Delete;
using Stowaway.Application.Modules.Sales.Order.Commands.Update;
using Stowaway.Application.Modules.Sales.Order.Queries.GetById;
using Stowaway.Application.Modules.Sales.Order.Queries.List;

namespace Stowaway.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController(ISender sender) : ControllerBase
    {
        [HttpPost]
        [HasPermission(Permissions.OrderCreate)]
        public async Task<CreateOrderCommandDto> CreateOrder(CreateOrderCommand command, CancellationToken ct)
        {
            CreateOrderCommandDto result = await sender.Send(command, ct);
            return result;
        }

        [HttpGet]
        [HasPermission(Permissions.OrderRead)]
        public async Task<PageResult<ListOrdersQueryDto>> ListOrders([FromQuery]ListOrdersQuery command, CancellationToken ct)
        {
            return await sender.Send(command, ct);
        }
        [HttpGet("{id:int}")]
        [HasPermission(Permissions.OrderRead)]
        public async Task<GetOrderByIdQueryDto> GetOrderById(int id, CancellationToken ct)
        {
            return await sender.Send(new GetOrderByIdQuery { Id = id}, ct);
        }
        [HttpPut]
        [HasPermission(Permissions.OrderUpdate)]
        public async Task<bool> UpdateUser([FromBody] UpdateOrderCommand command, CancellationToken ct)
        {
            return await sender.Send(command, ct);
        }

        [HttpDelete("{id:int}")]
        [HasPermission(Permissions.OrderDelete)]
        public async Task<bool> DeleteUser(int id, CancellationToken ct)
        {
            return await sender.Send(new DeleteOrderCommand { Id=id}, ct);
        }
    }
}
