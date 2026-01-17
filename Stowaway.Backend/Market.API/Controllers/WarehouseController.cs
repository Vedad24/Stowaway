using Stowaway.Application.Modules.Storage.Warehouse.Commands.Create;
using Stowaway.Application.Modules.Storage.Warehouse.Commands.Delete;
using Stowaway.Application.Modules.Storage.Warehouse.Commands.Update;
using Stowaway.Application.Modules.Storage.Warehouse.Queries.GetById;
using Stowaway.Application.Modules.Storage.Warehouse.Queries.List;

namespace Stowaway.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize(Policy = "AdminOnly")]
    public class WarehouseController(ISender sender) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<PageResult<ListWarehouseQueryDto>> List([FromQuery] ListWarehouseQuery query, CancellationToken ct)
        {
            var result = await sender.Send(query, ct);
            return result;
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<GetWarehouseQueryByIdDto> GetById(int id, CancellationToken ct)
        {
            var result = await sender.Send(new GetWarehouseByIdQuery { Id  = id }, ct);
            return result;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<int>> CreateWarehouse(CreateWarehouseCommand command, CancellationToken ct)
        {
            int id = await sender.Send(command, ct);
            return CreatedAtAction(nameof(GetById), new {id}, new {id});
        }

        [HttpDelete("{id:int}")]
        [AllowAnonymous]
        public async Task Delete(int id, CancellationToken ct)
        {
            await sender.Send(new DeleteWarehouseCommand { Id = id}, ct);
        }

        [HttpPut("{id:int}")]
        [AllowAnonymous]
        public async Task Update(int id, UpdateWarehouseCommand command, CancellationToken ct)
        {
            command.Id = id;
            await sender.Send(command, ct);
        }
    }
}
