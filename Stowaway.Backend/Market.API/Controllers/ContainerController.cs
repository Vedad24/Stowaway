using Market.API.Authorization;
using Market.Shared.Constants;
using Stowaway.Application.Modules.Storage.Container.Commands.Create;
using Stowaway.Application.Modules.Storage.Container.Commands.Delete;
using Stowaway.Application.Modules.Storage.Container.Commands.Move;
using Stowaway.Application.Modules.Storage.Container.Commands.Update;
using Stowaway.Application.Modules.Storage.Container.Commands.UpdateCanvasPosition;
using Stowaway.Application.Modules.Storage.Container.Commands.UpdateStatus;
using Stowaway.Application.Modules.Storage.Container.Queries.GetById;
using Stowaway.Application.Modules.Storage.Container.Shared;
using Stowaway.Application.Modules.Storage.Items.Queries.List;

namespace Stowaway.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ContainerController(ISender sender) : ControllerBase
    {
        [HttpGet]
        [HasPermission(Permissions.ContainerRead)]
        public async Task<ActionResult<ListContainersDto>> ListNames([FromQuery] ListContainersQuery query, CancellationToken cancellationToken)
        {
            var result = await sender.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [HasPermission(Permissions.ContainerRead)]
        [HasPriviledge(Priviledges.ContainerRead, WarehouseResolutionStrategy.ContainerRouteId)]
        public async Task<ActionResult<ListContainersDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetContainerByIdQuery { Id = id }, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [HasPermission(Permissions.ContainerCreate)]
        [HasPriviledge(Priviledges.ContainerCreate, WarehouseResolutionStrategy.BodyField, bodyFieldName: "WarehouseId")]
        public async Task<ActionResult<int>> Create(CreateContainerCommand command, CancellationToken cancellationToken)
        {
            int id = await sender.Send(command, cancellationToken);
            return Ok(id);
        }

        [HttpPut("{id:int}")]
        [HasPermission(Permissions.ContainerUpdate)]
        [HasPriviledge(Priviledges.ContainerUpdate, WarehouseResolutionStrategy.ContainerRouteId)]
        public async Task Update(int id, UpdateContainerCommand command, CancellationToken cancellationToken)
        {
            command.Id = id;
            await sender.Send(command, cancellationToken);
        }

        [HttpPut("{id:int}/canvas-position")]
        [HasPermission(Permissions.ContainerUpdate)]
        [HasPriviledge(Priviledges.ContainerUpdate, WarehouseResolutionStrategy.ContainerRouteId)]
        public async Task UpdateCanvasPosition(int id, UpdateContainerCanvasPositionCommand payload, CancellationToken cancellationToken)
        {
            payload.Id = id;
            await sender.Send(payload, cancellationToken);
        }

        // Only checks the source warehouse's priviledge; Move is itself constrained by the
        // handler to keep a container within the same warehouse, so this is sufficient.
        [HttpPut("{id:int}/parent-container")]
        [HasPermission(Permissions.ContainerUpdate)]
        [HasPriviledge(Priviledges.ContainerUpdate, WarehouseResolutionStrategy.ContainerRouteId)]
        public async Task Move(int id, MoveContainerCommand payload, CancellationToken cancellationToken)
        {
            payload.Id = id;
            await sender.Send(payload, cancellationToken);
        }

        [HttpPut("{id:int}/status")]
        [HasPermission(Permissions.ContainerUpdate)]
        [HasPriviledge(Priviledges.ContainerUpdate, WarehouseResolutionStrategy.ContainerRouteId)]
        public async Task UpdateStatus(int id, UpdateContainerStatusCommand payload, CancellationToken cancellationToken)
        {
            payload.Id = id;
            await sender.Send(payload, cancellationToken);
        }

        [HttpDelete("{id:int}")]
        [HasPermission(Permissions.ContainerDelete)]
        [HasPriviledge(Priviledges.ContainerDelete, WarehouseResolutionStrategy.ContainerRouteId)]
        public async Task Delete(
            int id,
            [FromQuery] bool deleteContents,
            [FromQuery] int? moveContentsToContainerId,
            CancellationToken cancellationToken)
        {
            await sender.Send(new DeleteContainerCommand
            {
                Id = id,
                DeleteContents = deleteContents,
                MoveContentsToContainerId = moveContentsToContainerId,
            }, cancellationToken);
        }
    }
}
