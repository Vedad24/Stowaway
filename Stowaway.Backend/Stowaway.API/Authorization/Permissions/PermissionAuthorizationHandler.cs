using Stowaway.Infrastructure.Database;
using Stowaway.Shared.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Stowaway.API.Authorization;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.HasClaim(c => c.Type == Permissions.ClaimType && (Permissions.AuthPrefix + c.Value) == requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
