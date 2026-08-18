using Market.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Market.API.Authorization;

public sealed class StowawayAuthPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

    public StowawayAuthPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy?> GetDefaultPolicyAsync() => _fallbackPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallbackPolicyProvider.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (string.IsNullOrWhiteSpace(policyName))
            return _fallbackPolicyProvider.GetPolicyAsync(policyName);
        if(policyName.StartsWith(Priviledges.AuthPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var parts = policyName.Split('|');
            var priviledge = parts[0];
            var strategy = parts.Length > 1 && Enum.TryParse<WarehouseResolutionStrategy>(parts[1], out var parsedStrategy)
                ? parsedStrategy
                : WarehouseResolutionStrategy.RouteId;
            var routeKey = parts.Length > 2 ? parts[2] : "id";
            var bodyFieldName = parts.Length > 3 ? parts[3] : "WarehouseId";

            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new PriviledgeRequirement(priviledge, strategy, routeKey, bodyFieldName))
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }else if(policyName.StartsWith(Permissions.AuthPrefix, StringComparison.OrdinalIgnoreCase))
        {
             var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }else
            return _fallbackPolicyProvider.GetPolicyAsync(policyName);
        
    }
}
