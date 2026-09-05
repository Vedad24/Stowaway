using Market.Application.Common.Exceptions;
using Market.Domain.Entities.Identity;
using Market.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Stowaway.Application.Modules.Identity.Users.Commands.Create;
using Stowaway.Domain.Entities.Identity;

namespace Market.Tests.UnitTests;

public class CreateUserCommandHandlerTests
{
    private static CreateUserCommand BuildCommand(RoleEntity? role = null) => new()
    {
        Email = "new.user@example.com",
        Password = "Password123!",
        FirstName = "New",
        LastName = "User",
        Role = role
    };

    [Fact]
    public async Task Handle_NoRoleRequested_AssignsUserRole_ForAnonymousCaller()
    {
        await using var db = TestDbContext.Create();
        var currentUser = new FakeCurrentUser { IsAuthenticated = false };
        var handler = new CreateUserCommandHandler(db, currentUser, new PasswordHasher<UserEntity>());

        var id = await handler.Handle(BuildCommand(), CancellationToken.None);

        var created = await db.Users.FindAsync(id);
        Assert.NotNull(created);
        Assert.Equal(Role.User, created!.RoleId);
    }

    [Fact]
    public async Task Handle_AnonymousCallerRequestsAdminRole_ThrowsUnauthorized()
    {
        await using var db = TestDbContext.Create();
        var currentUser = new FakeCurrentUser { IsAuthenticated = false };
        var handler = new CreateUserCommandHandler(db, currentUser, new PasswordHasher<UserEntity>());

        await Assert.ThrowsAsync<StowawayUnauthorizedException>(() =>
            handler.Handle(BuildCommand(new RoleEntity { Id = Role.Admin }), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ManagerCallerRequestsAdminRole_ThrowsUnauthorized()
    {
        await using var db = TestDbContext.Create();
        var currentUser = new FakeCurrentUser { IsManager = true };
        var handler = new CreateUserCommandHandler(db, currentUser, new PasswordHasher<UserEntity>());

        await Assert.ThrowsAsync<StowawayUnauthorizedException>(() =>
            handler.Handle(BuildCommand(new RoleEntity { Id = Role.Admin }), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_AdminCallerRequestsManagerRole_Succeeds()
    {
        await using var db = TestDbContext.Create();
        var currentUser = new FakeCurrentUser { IsAdmin = true };
        var handler = new CreateUserCommandHandler(db, currentUser, new PasswordHasher<UserEntity>());

        var id = await handler.Handle(BuildCommand(new RoleEntity { Id = Role.Manager }), CancellationToken.None);

        var created = await db.Users.FindAsync(id);
        Assert.NotNull(created);
        Assert.Equal(Role.Manager, created!.RoleId);
    }
}
