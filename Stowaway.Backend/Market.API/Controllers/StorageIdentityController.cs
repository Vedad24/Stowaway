using Market.API.Authorization;
using Market.Application.Modules.Storage.Priviledges.Queries.List;
using Market.Shared.Constants;
using Stowaway.Application.Modules.Storage.StorageIdentity.Commands.Create;
using Stowaway.Application.Modules.Storage.StorageIdentity.Commands.CreateUpdate;
using Stowaway.Application.Modules.Storage.StorageIdentity.Commands.Delete;
using Stowaway.Application.Modules.Storage.StorageIdentity.Commands.Update;
using Stowaway.Application.Modules.Storage.StorageIdentity.Queries.List;

namespace Stowaway.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class StorageIdentityController(ISender sender) : ControllerBase
    {
        [HttpGet("privilege-groups")]
        [HasPermission(Permissions.WarehouseUsersManage)]
        public async Task<ActionResult<List<ListPriviledgeGroupQueryDto>>> ListPrivilegeGroups([FromQuery] int? warehouseId, CancellationToken ct)
        {
            var result = await sender.Send(new ListPriviledgeGroupsQuery { WarehouseId = warehouseId }, ct);
            return Ok(result);
        }

        [HttpPost("privilege-groups")]
        [HasPermission(Permissions.WarehouseUsersManage)]
        public async Task<ActionResult<int>> CreatePrivilegeGroup(CreatePriviledgeGroupCommand command, CancellationToken ct)
        {
            var id = await sender.Send(command, ct);
            return CreatedAtAction(nameof(ListPrivilegeGroups), new { id }, new { id });
        }

        [HttpPut("privilege-groups/{privilegeId}")]
        [HasPermission(Permissions.WarehouseUsersManage)]
        public async Task<ActionResult<int>> UpdatePrivilegeGroup(int privilegeId, UpdatePriviledgeGroupCommand command, CancellationToken ct)
        {
            command.PriviledgeId = privilegeId;
            var id = await sender.Send(command, ct);
            return Ok(new { id });
        }

        [HttpDelete("privilege-groups/{privilegeId}")]
        [HasPermission(Permissions.WarehouseUsersManage)]
        public async Task<ActionResult> DeletePrivilegeGroup(int privilegeId, CancellationToken ct)
        {
            await sender.Send(new DeletePriviledgeGroupCommand { Id = privilegeId }, ct);
            return Ok();
        }

        [HttpGet("privileges")]
        [AllowAnonymous] //this should stay anon, but probably cached instead of hitting the db always
        public async Task<ActionResult<List<ListPriviledgesQueryDto>>> ListPrivileges(CancellationToken ct)
        {
            var result = await sender.Send(new ListPriviledgesQuery(), ct);
            return Ok(result);
        }

        [HttpPut("warehouse-users")]
        [HasPermission(Permissions.WarehouseUsersManage)]
        public async Task<ActionResult<CreateUpdateWarehouseUserCommandDto>> CreateUpdateWarehouseUser(CreateUpdateWarehouseUserCommand command, CancellationToken ct)
        {
            var result = await sender.Send(command, ct);
            return Ok(result);
        }
    }
}
