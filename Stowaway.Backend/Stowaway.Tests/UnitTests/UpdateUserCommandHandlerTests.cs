using Stowaway.Application.Common.Exceptions;
using Stowaway.Domain.Entities.Identity;
using Stowaway.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Stowaway.Application.Modules.Identity.Users.Commands.Update;
using Stowaway.Domain.Entities.Identity;

namespace Stowaway.Tests.UnitTests;

public class UpdateUserCommandHandlerTests
{
    private static async Task<UserEntity> SeedUser(TestDbContext db, Role role = Role.User)
    {
        db.Roles.Add(new RoleEntity { Id = role });
        var user = new UserEntity
        {
            Email = "target@example.com",
            PasswordHash = "hash",
            FirstName = "Target",
            LastName = "User",
            RoleId = role,
            IsEnabled = true
        };
        db.Users.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);
        return user;
    }

    private static UpdateUserCommand BuildCommand(int id, RoleEntity? role) => new()
    {
        Id = id,
        Email = null,
        Password = null,
        FirstName = null,
        LastName = null,
        Role = role,
        IsEnabled = null
    };

    [Fact]
    public async Task Handle_ManagerCaller_CanPromoteUserToManager()
    {
        await using var db = TestDbContext.Create();
        var user = await SeedUser(db);
        db.Roles.Add(new RoleEntity { Id = Role.Manager });
        await db.SaveChangesAsync(CancellationToken.None);
        db.ChangeTracker.Clear();

        var currentUser = new FakeCurrentUser { IsManager = true };
        var handler = new UpdateUserCommandHandler(db, currentUser, new PasswordHasher<UserEntity>());

        var result = await handler.Handle(BuildCommand(user.Id, new RoleEntity { Id = Role.Manager }), CancellationToken.None);

        Assert.Equal(Role.Manager, result.Role!.Id);
    }

    [Fact]
    public async Task Handle_ManagerCaller_CannotAssignAdminRole()
    {
        await using var db = TestDbContext.Create();
        var user = await SeedUser(db);
        db.Roles.Add(new RoleEntity { Id = Role.Admin });
        await db.SaveChangesAsync(CancellationToken.None);

        var currentUser = new FakeCurrentUser { IsManager = true };
        var handler = new UpdateUserCommandHandler(db, currentUser, new PasswordHasher<UserEntity>());

        await Assert.ThrowsAsync<StowawayUnauthorizedException>(() =>
            handler.Handle(BuildCommand(user.Id, new RoleEntity { Id = Role.Admin }), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_AdminCaller_CanAssignAdminRole()
    {
        await using var db = TestDbContext.Create();
        var user = await SeedUser(db);
        db.Roles.Add(new RoleEntity { Id = Role.Admin });
        await db.SaveChangesAsync(CancellationToken.None);
        db.ChangeTracker.Clear();

        var currentUser = new FakeCurrentUser { IsAdmin = true };
        var handler = new UpdateUserCommandHandler(db, currentUser, new PasswordHasher<UserEntity>());

        var result = await handler.Handle(BuildCommand(user.Id, new RoleEntity { Id = Role.Admin }), CancellationToken.None);

        Assert.Equal(Role.Admin, result.Role!.Id);
    }

    [Fact]
    public async Task Handle_AdminCaller_CanUpdateExistingAdminUser()
    {
        await using var db = TestDbContext.Create();
        var user = await SeedUser(db, Role.Admin);
        await db.SaveChangesAsync(CancellationToken.None);
        db.ChangeTracker.Clear();

        var currentUser = new FakeCurrentUser { IsAdmin = true };
        var handler = new UpdateUserCommandHandler(db, currentUser, new PasswordHasher<UserEntity>());

        var command = BuildCommand(user.Id, null);
        command.FirstName = "Updated";
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("Updated", result.FirstName);
    }

    [Fact]
    public async Task Handle_ManagerCaller_CannotUpdateExistingAdminUser()
    {
        await using var db = TestDbContext.Create();
        var user = await SeedUser(db, Role.Admin);
        await db.SaveChangesAsync(CancellationToken.None);

        var currentUser = new FakeCurrentUser { IsManager = true };
        var handler = new UpdateUserCommandHandler(db, currentUser, new PasswordHasher<UserEntity>());

        var command = BuildCommand(user.Id, null);
        command.FirstName = "Updated";

        await Assert.ThrowsAsync<StowawayUnauthorizedException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}
