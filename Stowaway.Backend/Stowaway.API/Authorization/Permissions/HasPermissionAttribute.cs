using Stowaway.Shared.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Stowaway.API.Authorization;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
    {
        Policy = $"{Permissions.AuthPrefix}{permission}";
    }
}
