using Microsoft.AspNetCore.Authorization;

namespace Market.API.Authorization;

public sealed class PriviledgeRequirement : IAuthorizationRequirement
{
    public string Priviledge { get; }

    public PriviledgeRequirement(string priviledge)
    {
        Priviledge = priviledge ?? throw new ArgumentNullException(nameof(priviledge));
    }
}
