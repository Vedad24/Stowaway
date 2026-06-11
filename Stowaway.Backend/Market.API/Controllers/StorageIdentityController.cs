using Market.API.Authorization;
using Stowaway.Application.Modules.Storage.StorageIdentity.Commands.Create;
using Stowaway.Application.Modules.Storage.StorageIdentity.Queries.List;

namespace Stowaway.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StorageIdentityController(ISender sender) : ControllerBase
    {
        [HttpGet("privilege-groups")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ListPriviledgeGroupQueryDto>>> ListPrivilegeGroups([FromQuery] int? warehouseId, CancellationToken ct)
        {
            var result = await sender.Send(new ListPriviledgeGroupsQuery { WarehouseId = warehouseId }, ct);
            return Ok(result);
        }

        [HttpPost("privilege-groups")]
        public async Task<ActionResult<int>> CreatePrivilegeGroup(CreatePriviledgeGroupCommand command, CancellationToken ct)
        {
            var id = await sender.Send(command, ct);
            return CreatedAtAction(nameof(ListPrivilegeGroups), new { id }, new { id });
        }
    }
}
