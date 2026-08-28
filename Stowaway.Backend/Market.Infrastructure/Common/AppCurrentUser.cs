using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Market.Application.Abstractions;
using Market.Shared.Constants;
using Stowaway.Domain.Entities.Identity;

namespace Market.Infrastructure.Common;

/// <summary>
/// Implementation of IAppCurrentUser that reads data from a JWT token.
/// </summary>
public sealed class AppCurrentUser(IHttpContextAccessor httpContextAccessor)
    : IAppCurrentUser
{
    private readonly ClaimsPrincipal? _user = httpContextAccessor.HttpContext?.User;

    public int? UserId =>
        int.TryParse(_user?.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : null;

    public string? Email =>
        _user?.FindFirstValue(ClaimTypes.Email);

    public bool IsAuthenticated =>
        _user?.Identity?.IsAuthenticated ?? false;

    private Role? RoleClaim =>
        Enum.TryParse<Role>(_user?.FindFirstValue(ClaimTypes.Role), out var role) ? role : null;

    public bool IsAdmin => RoleClaim == Role.Admin;

    public bool IsManager => RoleClaim == Role.Manager;

    public bool IsEmployee => RoleClaim == Role.User;

    public bool HasPermission(string permission) =>
        _user?.HasClaim(c => c.Type == Permissions.ClaimType && c.Value == permission) ?? false;

}