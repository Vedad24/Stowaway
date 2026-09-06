using Stowaway.API.Authorization;
using Stowaway.Shared.Constants;
using Stowaway.Application.Modules.Storage.Items.Shared;
using Stowaway.Application.Modules.Storage.Tags.Commands.Create;
using Stowaway.Application.Modules.Storage.Tags.Queries.List;

namespace Stowaway.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class TagController(ISender sender) : ControllerBase
    {
        [HttpGet]
        [HasPermission(Permissions.TagRead)]
        public async Task<ActionResult<List<SharedTagDto>>> List(CancellationToken cancellationToken)
        {
            var result = await sender.Send(new ListTagsQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [HasPermission(Permissions.TagCreate)]
        public async Task<ActionResult<SharedTagDto>> Create(CreateTagCommand command, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            return Ok(result);
        }
    }
}
