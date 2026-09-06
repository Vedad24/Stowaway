using Stowaway.Shared.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Stowaway.API.Authorization;

public sealed class HasPriviledgeAttribute : AuthorizeAttribute
{
    // Policy string carries the priviledge code plus warehouse-resolution metadata,
    // pipe-delimited, so StowawayAuthPolicyProvider can rebuild a full PriviledgeRequirement
    // from just the policy name without a separate out-of-band lookup.
    public HasPriviledgeAttribute(
        string priviledge,
        WarehouseResolutionStrategy strategy = WarehouseResolutionStrategy.RouteId,
        string routeKey = "id",
        string bodyFieldName = "WarehouseId")
    {
        Policy = $"{Priviledges.AuthPrefix}{priviledge}|{strategy}|{routeKey}|{bodyFieldName}";
    }
}
