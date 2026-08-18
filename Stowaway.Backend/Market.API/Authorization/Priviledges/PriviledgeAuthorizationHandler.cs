using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Market.Infrastructure.Database;
using Market.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Market.API.Authorization;

public sealed class PriviledgeAuthorizationHandler(DatabaseContext dbContext) : AuthorizationHandler<PriviledgeRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PriviledgeRequirement requirement)
    {
        var userIdString = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdString, out var userId))
            return;

        var warehouseId = await GetWarehouseIdAsync(context, requirement);
        if (warehouseId is null)
            return;

        string priviledgeCode = requirement.Priviledge;
        var hasPriviledge = await dbContext.WarehouseUsers
            .Where(wu => wu.UserId == userId && wu.WarehouseId == warehouseId)
            .SelectMany(wu => wu.PriviledgeGroup.Priviledges)
            .AnyAsync(p => Priviledges.AuthPrefix + p.Priviledge.Code == priviledgeCode);

        if (hasPriviledge)
            context.Succeed(requirement);
    }

    private async Task<int?> GetWarehouseIdAsync(AuthorizationHandlerContext context, PriviledgeRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext)
            return null;

        switch (requirement.Strategy)
        {
            case WarehouseResolutionStrategy.RouteId:
                return ParseRouteInt(httpContext, requirement.RouteKey);

            case WarehouseResolutionStrategy.ContainerRouteId:
            {
                var containerId = ParseRouteInt(httpContext, requirement.RouteKey);
                if (containerId is null)
                    return null;
                return await dbContext.Containers
                    .Where(c => c.Id == containerId)
                    .Select(c => (int?)c.WarehouseId)
                    .FirstOrDefaultAsync();
            }

            case WarehouseResolutionStrategy.ItemRouteId:
            {
                var itemId = ParseRouteInt(httpContext, requirement.RouteKey);
                if (itemId is null)
                    return null;
                return await dbContext.Item
                    .Where(i => i.Id == itemId)
                    .Select(i => (int?)i.Container.WarehouseId)
                    .FirstOrDefaultAsync();
            }

            case WarehouseResolutionStrategy.BodyField:
                return await ReadIntFromBodyAsync(httpContext, requirement.BodyFieldName);

            case WarehouseResolutionStrategy.BodyFieldViaContainer:
            {
                var containerId = await ReadIntFromBodyAsync(httpContext, "ContainerId");
                if (containerId is null)
                    return null;
                return await dbContext.Containers
                    .Where(c => c.Id == containerId)
                    .Select(c => (int?)c.WarehouseId)
                    .FirstOrDefaultAsync();
            }

            default:
                return null;
        }
    }

    private static int? ParseRouteInt(HttpContext httpContext, string routeKey)
    {
        var value = httpContext.Request.RouteValues[routeKey]?.ToString();
        return int.TryParse(value, out var parsed) ? parsed : null;
    }

    // Authorization runs before MVC model binding, so the request body is still unread here.
    // Buffer it, peek the field we need, then rewind so the controller's own model binder
    // can read it again from the start.
    private static async Task<int?> ReadIntFromBodyAsync(HttpContext httpContext, string fieldName)
    {
        httpContext.Request.EnableBuffering();
        httpContext.Request.Body.Position = 0;

        using var document = await JsonDocument.ParseAsync(
            httpContext.Request.Body,
            new JsonDocumentOptions { AllowTrailingCommas = true },
            httpContext.RequestAborted);

        httpContext.Request.Body.Position = 0;

        foreach (var property in document.RootElement.EnumerateObject())
        {
            if (string.Equals(property.Name, fieldName, StringComparison.OrdinalIgnoreCase)
                && property.Value.TryGetInt32(out var value))
            {
                return value;
            }
        }

        return null;
    }
}
