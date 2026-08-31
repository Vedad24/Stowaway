using Market.API.Authorization;
using Market.Shared.Constants;
using Stowaway.Application.Modules.Storage.Items.Commands.Create;
using Stowaway.Application.Modules.Storage.Items.Commands.Delete;
using Stowaway.Application.Modules.Storage.Items.Commands.Move;
using Stowaway.Application.Modules.Storage.Items.Commands.Update;
using Stowaway.Application.Modules.Storage.Items.Commands.UpdateCanvasPosition;
using Stowaway.Application.Modules.Storage.Items.Queries.GetById;
using Stowaway.Application.Modules.Storage.Items.Queries.List;

namespace Stowaway.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ItemController(ISender sender) : ControllerBase
    {
        [HttpGet]
        [HasPermission(Permissions.ItemRead)]
        public async Task<PageResult<ListItemQueryDto>> List([FromQuery] ListItemQuery query, CancellationToken cancellationToken)
        {
            var result = await sender.Send(query, cancellationToken);
            return result;
        }

        [HttpGet("{id:int}")]
        [HasPermission(Permissions.ItemRead)]
        [HasPriviledge(Priviledges.ItemRead, WarehouseResolutionStrategy.ItemRouteId)]
        public async Task<GetItemByIdQueryDto> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetItemByIdQuery { Id = id },cancellationToken);
            return result;
        }

        [HttpPost]
        [HasPermission(Permissions.ItemCreate)]
        [HasPriviledge(Priviledges.ItemCreate, WarehouseResolutionStrategy.BodyFieldViaContainer)]
        public async Task<ActionResult<int>> Create(CreateItemCommand payload, CancellationToken cancellationToken)
        {
            int id = await sender.Send(payload, cancellationToken);
            return CreatedAtAction(nameof(GetById), new {id}, new {id});
        }

        [HttpDelete("{id:int}")]
        [HasPermission(Permissions.ItemDelete)]
        [HasPriviledge(Priviledges.ItemDelete, WarehouseResolutionStrategy.ItemRouteId)]
        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            await sender.Send(new DeleteItemCommand { Id = id},cancellationToken);
        }

        [HttpPut("{id:int}")]
        [HasPermission(Permissions.ItemUpdate)]
        [HasPriviledge(Priviledges.ItemUpdate, WarehouseResolutionStrategy.ItemRouteId)]
        public async Task Update(int id, UpdateItemCommand payload, CancellationToken cancellationToken)
        {
            payload.Id = id;
            await sender.Send(payload, cancellationToken);
        }

        [HttpPut("{id:int}/canvas-position")]
        [HasPermission(Permissions.ItemUpdate)]
        [HasPriviledge(Priviledges.ItemUpdate, WarehouseResolutionStrategy.ItemRouteId)]
        public async Task UpdateCanvasPosition(int id, UpdateItemCanvasPositionCommand payload, CancellationToken cancellationToken)
        {
            payload.Id = id;
            await sender.Send(payload, cancellationToken);
        }

        // Only checks the source warehouse's priviledge; MoveItemCommandHandler rejects the
        // move if the destination container is in a different warehouse, so this is sufficient
        // (matches Container.Move's same-warehouse constraint).
        [HttpPut("{id:int}/container")]
        [HasPermission(Permissions.ItemUpdate)]
        [HasPriviledge(Priviledges.ItemUpdate, WarehouseResolutionStrategy.ItemRouteId)]
        public async Task Move(int id, MoveItemCommand payload, CancellationToken cancellationToken)
        {
            payload.Id = id;
            await sender.Send(payload, cancellationToken);
        }
    }
}
