using Stowaway.Application.Modules.Storage.Items.Shared;
using Stowaway.Application.Modules.Storage.Tags.Commands.Create;
using Stowaway.Application.Modules.Storage.Tags.Queries.List;

namespace Stowaway.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TagController(ISender sender) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<SharedTagDto>>> List(CancellationToken cancellationToken)
        {
            var result = await sender.Send(new ListTagsQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<SharedTagDto>> Create(CreateTagCommand command, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            return Ok(result);
        }
    }
}
