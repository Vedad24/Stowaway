using Stowaway.Application.Abstractions;

namespace Stowaway.Tests.Helpers;

public sealed class FakeCurrentUser : IAppCurrentUser
{
    public int? UserId { get; init; }
    public string? Email { get; init; }
    public bool IsAuthenticated { get; init; } = true;
    public bool IsAdmin { get; init; }
    public bool IsManager { get; init; }
    public bool IsEmployee { get; init; }
    public HashSet<string> Permissions { get; init; } = [];

    public bool HasPermission(string permission) => Permissions.Contains(permission);

    public IReadOnlyList<string> PermissionCodes => Permissions.ToList();
}
