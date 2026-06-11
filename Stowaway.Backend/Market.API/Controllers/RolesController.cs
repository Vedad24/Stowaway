using Market.API.Authorization;
using Market.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stowaway.Application.Modules.Identity.Roles.Queries.List;

namespace Stowaway.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController(ISender sender) : ControllerBase
    {
        [HttpGet]
        [HasPermission(Permissions.RolesRead)]
        public async Task<List<ListRolesQueryDto>> GetRoles([FromQuery]ListRolesQuery command, CancellationToken ct)
        {
            return await sender.Send(command, ct);
        }

    }
}
