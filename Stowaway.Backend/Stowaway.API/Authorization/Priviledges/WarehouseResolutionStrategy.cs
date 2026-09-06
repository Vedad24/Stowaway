namespace Stowaway.API.Authorization;

public enum WarehouseResolutionStrategy
{
    // Route value (named by RouteKey) IS the warehouse id.
    RouteId,

    // Route value (named by RouteKey) is a Container id -> look up Container.WarehouseId.
    ContainerRouteId,

    // Route value (named by RouteKey) is an Item id -> look up Item.Container.WarehouseId.
    ItemRouteId,

    // Route value (named by RouteKey) is a PriviledgeGroup id -> look up PriviledgeGroup.WarehouseId.
    PriviledgeGroupRouteId,

    // An int field (named by BodyFieldName) on the JSON request body IS the warehouse id.
    BodyField,

    // An int "ContainerId" field on the JSON request body -> look up Container.WarehouseId.
    BodyFieldViaContainer,

    // A query-string value (named by RouteKey) IS the warehouse id.
    QueryStringField,
}
