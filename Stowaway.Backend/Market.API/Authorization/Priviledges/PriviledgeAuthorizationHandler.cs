using System.Net.Http.Headers;
using System.Security.Claims;
using Market.Infrastructure.Database;
using Market.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Market.API.Authorization;

public sealed class PriviledgeAuthorizationHandler(DatabaseContext dbContext) : AuthorizationHandler<PriviledgeRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PriviledgeRequirement requirement)
    {
        
        var userIdString = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int userId;
        if(!int.TryParse(userIdString, out userId))
            return;
        int? warehouseId = GetWarehouseId(context);
        if(warehouseId is null)
            return;
        var hasPriviledge = await dbContext.WarehouseUsers
        .Where(wu => wu.UserId == userId && wu.WarehouseId == warehouseId)
        .SelectMany(wu => wu.PriviledgeGroup.Priviledges) 
        .AnyAsync(p => p.Code == requirement.Priviledge);
        if(hasPriviledge)
            context.Succeed(requirement);
        
        return;

    }

    private int? GetWarehouseId(AuthorizationHandlerContext context)
    {
        if(context.Resource is not AuthorizationFilterContext mvcContext)
            return null;
        //Change id to warehouseId after controller update
        var warehouseIdValue = mvcContext.RouteData.Values["id"]?.ToString();
        if(!int.TryParse(warehouseIdValue, out var warehouseId))
            return null;
        return warehouseId;
    }
}
