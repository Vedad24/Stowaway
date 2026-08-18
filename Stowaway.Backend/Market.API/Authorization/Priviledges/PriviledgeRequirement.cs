using Microsoft.AspNetCore.Authorization;

namespace Market.API.Authorization;

public sealed class PriviledgeRequirement : IAuthorizationRequirement
{
    public string Priviledge { get; }
    public WarehouseResolutionStrategy Strategy { get; }
    public string RouteKey { get; }
    public string BodyFieldName { get; }

    public PriviledgeRequirement(
        string priviledge,
        WarehouseResolutionStrategy strategy = WarehouseResolutionStrategy.RouteId,
        string routeKey = "id",
        string bodyFieldName = "WarehouseId")
    {
        Priviledge = priviledge ?? throw new ArgumentNullException(nameof(priviledge));
        Strategy = strategy;
        RouteKey = routeKey;
        BodyFieldName = bodyFieldName;
    }
}
