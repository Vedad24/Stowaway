using Stowaway.API.Authorization;
using Stowaway.Shared.Constants;
using Stowaway.Application.Modules.Storage.Warehouse.Commands.Create;
using Stowaway.Application.Modules.Storage.Warehouse.Commands.Delete;
using Stowaway.Application.Modules.Storage.Warehouse.Commands.Update;
using Stowaway.Application.Modules.Storage.Warehouse.Commands.UpdateName;
using Stowaway.Application.Modules.Storage.Warehouse.Queries.GetById;
using Stowaway.Application.Modules.Storage.Warehouse.Queries.List;

namespace Stowaway.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class WarehouseController(ISender sender) : ControllerBase
    {
        [HttpGet]
        [HasPermission(Permissions.WarehouseRead)]
        public async Task<PageResult<ListWarehouseQueryDto>> List([FromQuery] ListWarehouseQuery query, CancellationToken ct)
        {
            var result = await sender.Send(query, ct);
            return result;
        }

        [HttpGet("{id:int}")]
        [HasPermission(Permissions.WarehouseRead)]
        public async Task<GetWarehouseQueryByIdDto> GetById(int id, CancellationToken ct)
        {
            var result = await sender.Send(new GetWarehouseByIdQuery { Id  = id }, ct);
            return result;
        }

        [HttpPost]
        [HasPermission(Permissions.WarehouseCreate)]
        public async Task<ActionResult<int>> CreateWarehouse(CreateWarehouseCommand command, CancellationToken ct)
        {
            int id = await sender.Send(command, ct);
            return CreatedAtAction(nameof(GetById), new {id}, new {id});
        }

        [HttpDelete("{id:int}")]
        [HasPermission(Permissions.WarehouseDelete)]
        [HasPriviledge(Priviledges.WarehouseDelete)]
        public async Task Delete(int id, CancellationToken ct)
        {
            await sender.Send(new DeleteWarehouseCommand { Id = id}, ct);
        }

        [HttpPut("{id:int}")]
        [HasPermission(Permissions.WarehouseUpdate)]
        [HasPriviledge(Priviledges.WarehouseUpdate)]
        public async Task Update(int id, UpdateWarehouseCommand command, CancellationToken ct)
        {
            command.Id = id;
            await sender.Send(command, ct);
        }

        [HttpPatch("{id:int}/name")]
        [HasPermission(Permissions.WarehouseUpdate)]
        [HasPriviledge(Priviledges.WarehouseUpdate)]
        public async Task<UpdateWarehouseNameCommandDto> UpdateName(int id, UpdateWarehouseNameCommand command, CancellationToken ct)
        {
            command.Id = id;
            return await sender.Send(command, ct);
        }
    }
}
