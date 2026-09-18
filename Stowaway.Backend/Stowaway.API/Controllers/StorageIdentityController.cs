using Stowaway.API.Authorization;
using Stowaway.Application.Modules.Storage.Priviledges.Queries.List;
using Stowaway.Shared.Constants;
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
        [HasPriviledge(Priviledges.WarehouseUsersManage, WarehouseResolutionStrategy.QueryStringField, routeKey: "warehouseId")]
        public async Task<ActionResult<PageResult<ListPriviledgeGroupQueryDto>>> ListPrivilegeGroups([FromQuery] ListPriviledgeGroupsQuery query, CancellationToken ct)
        {
            var result = await sender.Send(query, ct);
            return Ok(result);
        }

        [HttpPost("privilege-groups")]
        [HasPermission(Permissions.WarehouseUsersManage)]
        [HasPriviledge(Priviledges.WarehouseUsersManage, WarehouseResolutionStrategy.BodyField, bodyFieldName: "WarehouseId")]
        public async Task<ActionResult<int>> CreatePrivilegeGroup(CreatePriviledgeGroupCommand command, CancellationToken ct)
        {
            var id = await sender.Send(command, ct);
            return CreatedAtAction(nameof(ListPrivilegeGroups), new { id }, new { id });
        }

        [HttpPut("privilege-groups/{privilegeId}")]
        [HasPermission(Permissions.WarehouseUsersManage)]
        [HasPriviledge(Priviledges.WarehouseUsersManage, WarehouseResolutionStrategy.BodyField, bodyFieldName: "WarehouseId")]
        public async Task<ActionResult<int>> UpdatePrivilegeGroup(int privilegeId, UpdatePriviledgeGroupCommand command, CancellationToken ct)
        {
            command.PriviledgeId = privilegeId;
            var id = await sender.Send(command, ct);
            return Ok(new { id });
        }

        [HttpDelete("privilege-groups/{privilegeId}")]
        [HasPermission(Permissions.WarehouseUsersManage)]
        [HasPriviledge(Priviledges.WarehouseUsersManage, WarehouseResolutionStrategy.PriviledgeGroupRouteId, routeKey: "privilegeId")]
        public async Task<ActionResult> DeletePrivilegeGroup(int privilegeId, CancellationToken ct)
        {
            await sender.Send(new DeletePriviledgeGroupCommand { Id = privilegeId }, ct);
            return Ok();
        }

        [HttpGet("privileges")]
        [AllowAnonymous] //This should stay anonymous
        public async Task<ActionResult<List<ListPriviledgesQueryDto>>> ListPrivileges(CancellationToken ct)
        {
            var result = await sender.Send(new ListPriviledgesQuery(), ct);
            return Ok(result);
        }

        [HttpPut("warehouse-users")]
        [HasPermission(Permissions.WarehouseUsersManage)]
        [HasPriviledge(Priviledges.WarehouseUsersManage, WarehouseResolutionStrategy.BodyField, bodyFieldName: "WarehouseId")]
        public async Task<ActionResult<CreateUpdateWarehouseUserCommandDto>> CreateUpdateWarehouseUser(CreateUpdateWarehouseUserCommand command, CancellationToken ct)
        {
            var result = await sender.Send(command, ct);
            return Ok(result);
        }

        [HttpDelete("warehouse-users")]
        [HasPermission(Permissions.WarehouseUsersManage)]
        [HasPriviledge(Priviledges.WarehouseUsersManage, WarehouseResolutionStrategy.QueryStringField, routeKey: "warehouseId")]
        public async Task<ActionResult> DeleteWarehouseUser([FromQuery] DeleteWarehouseUserCommand command, CancellationToken ct)
        {
            await sender.Send(command, ct);
            return Ok();
        }
    }
}
