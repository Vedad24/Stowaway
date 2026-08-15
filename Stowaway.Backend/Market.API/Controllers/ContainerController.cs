using Stowaway.Application.Modules.Storage.Container.Commands.Create;
using Stowaway.Application.Modules.Storage.Container.Commands.Delete;
using Stowaway.Application.Modules.Storage.Container.Commands.Move;
using Stowaway.Application.Modules.Storage.Container.Commands.Update;
using Stowaway.Application.Modules.Storage.Container.Commands.UpdateCanvasPosition;
using Stowaway.Application.Modules.Storage.Container.Queries.GetById;
using Stowaway.Application.Modules.Storage.Container.Shared;
using Stowaway.Application.Modules.Storage.Items.Queries.List;

namespace Stowaway.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContainerController(ISender sender) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ListContainersDto>> ListNames([FromQuery] ListContainersQuery query, CancellationToken cancellationToken)
        {
            var result = await sender.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ListContainersDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetContainerByIdQuery { Id = id }, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<int>> Create(CreateContainerCommand command, CancellationToken cancellationToken)
        {
            int id = await sender.Send(command, cancellationToken);
            return Ok(id);
        }

        [HttpPut("{id:int}")]
        [AllowAnonymous]
        public async Task Update(int id, UpdateContainerCommand command, CancellationToken cancellationToken)
        {
            command.Id = id;
            await sender.Send(command, cancellationToken);
        }

        [HttpPut("{id:int}/canvas-position")]
        [AllowAnonymous]
        public async Task UpdateCanvasPosition(int id, UpdateContainerCanvasPositionCommand payload, CancellationToken cancellationToken)
        {
            payload.Id = id;
            await sender.Send(payload, cancellationToken);
        }

        [HttpPut("{id:int}/parent-container")]
        [AllowAnonymous]
        public async Task Move(int id, MoveContainerCommand payload, CancellationToken cancellationToken)
        {
            payload.Id = id;
            await sender.Send(payload, cancellationToken);
        }

        [HttpDelete("{id:int}")]
        [AllowAnonymous]
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
