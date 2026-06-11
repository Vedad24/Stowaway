using Market.Shared.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Market.API.Authorization;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
    {
        Policy = $"{Permissions.AuthPrefix}{permission}";
    }
}
