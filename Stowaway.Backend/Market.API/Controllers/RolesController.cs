using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stowaway.Application.Modules.Identity.Roles.Queries.List;

namespace Stowaway.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous] //delete later
    public class RolesController(ISender sender) : ControllerBase
    {
        [HttpGet]
        public async Task<List<ListRolesQueryDto>> GetRoles([FromQuery]ListRolesQuery command, CancellationToken ct)
        {
            return await sender.Send(command, ct);
        }

    }
}
