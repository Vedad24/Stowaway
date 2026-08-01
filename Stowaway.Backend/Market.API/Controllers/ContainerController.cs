using Stowaway.Application.Modules.Storage.Container.Commands.Move;
using Stowaway.Application.Modules.Storage.Container.Commands.UpdateCanvasPosition;
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
    }
}
