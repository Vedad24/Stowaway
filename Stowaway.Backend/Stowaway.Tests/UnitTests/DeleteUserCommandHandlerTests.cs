using Stowaway.Application.Common.Exceptions;
using Stowaway.Application.Modules.Identity.Users.Commands.Delete;
using Stowaway.Domain.Entities.Identity;
using Stowaway.Tests.Helpers;

namespace Stowaway.Tests.UnitTests;

public class DeleteUserCommandHandlerTests
{
    private static async Task<UserEntity> SeedUser(TestDbContext db, Role role)
    {
        if (!db.Roles.Any(r => r.Id == role))
            db.Roles.Add(new RoleEntity { Id = role });

        var user = new UserEntity
        {
            Email = "target@example.com",
            PasswordHash = "hash",
            RoleId = role,
            IsEnabled = true
        };
        db.Users.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);
        return user;
    }

    [Fact]
    public async Task Handle_AdminCaller_CanDeleteAdminUser()
    {
        await using var db = TestDbContext.Create();
        var user = await SeedUser(db, Role.Admin);

        var handler = new DeleteUserCommandHandler(db, new FakeCurrentUser { IsAdmin = true });

        var result = await handler.Handle(new DeleteUserCommand { Id = user.Id }, CancellationToken.None);

        Assert.True(result);
        Assert.False(await db.Users.AnyAsync(u => u.Id == user.Id));
    }

    [Fact]
    public async Task Handle_AdminCaller_CanDeleteRegularUser()
    {
        await using var db = TestDbContext.Create();
        var user = await SeedUser(db, Role.User);

        var handler = new DeleteUserCommandHandler(db, new FakeCurrentUser { IsAdmin = true });

        var result = await handler.Handle(new DeleteUserCommand { Id = user.Id }, CancellationToken.None);

        Assert.True(result);
        Assert.False(await db.Users.AnyAsync(u => u.Id == user.Id));
    }

    [Fact]
    public async Task Handle_ManagerCaller_CanDeleteRegularUser()
    {
        await using var db = TestDbContext.Create();
        var user = await SeedUser(db, Role.User);

        var handler = new DeleteUserCommandHandler(db, new FakeCurrentUser { IsManager = true });

        var result = await handler.Handle(new DeleteUserCommand { Id = user.Id }, CancellationToken.None);

        Assert.True(result);
        Assert.False(await db.Users.AnyAsync(u => u.Id == user.Id));
    }

    [Fact]
    public async Task Handle_ManagerCaller_CannotDeleteAdminUser()
    {
        await using var db = TestDbContext.Create();
        var user = await SeedUser(db, Role.Admin);

        var handler = new DeleteUserCommandHandler(db, new FakeCurrentUser { IsManager = true });

        await Assert.ThrowsAsync<StowawayUnauthorizedException>(() =>
            handler.Handle(new DeleteUserCommand { Id = user.Id }, CancellationToken.None));

        Assert.True(await db.Users.AnyAsync(u => u.Id == user.Id));
    }
}
