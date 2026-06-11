using Market.Shared.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Market.API.Authorization;

public sealed class HasPriviledgeAttribute : AuthorizeAttribute
{
    public HasPriviledgeAttribute(string priviledge)
    {
        Policy = $"{Priviledges.AuthPrefix}{priviledge}";
    }
}
