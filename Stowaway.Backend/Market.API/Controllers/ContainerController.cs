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
    }
}
