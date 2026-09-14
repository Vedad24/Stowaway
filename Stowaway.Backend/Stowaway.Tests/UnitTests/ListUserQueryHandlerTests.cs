using Stowaway.Application.Modules.Identity.Users.Queries.List;
using Stowaway.Domain.Entities.Identity;
using Stowaway.Tests.Helpers;

namespace Stowaway.Tests.UnitTests;

public class ListUserQueryHandlerTests
{
    private static async Task SeedUsers(TestDbContext db)
    {
        db.Roles.AddRange(
            new RoleEntity { Id = Role.User },
            new RoleEntity { Id = Role.Admin },
            new RoleEntity { Id = Role.Manager });

        db.Users.AddRange(
            new UserEntity { Id = 1, Email = "admin@test.com", PasswordHash = "x", RoleId = Role.Admin, IsEnabled = true },
            new UserEntity { Id = 2, Email = "manager@test.com", PasswordHash = "x", RoleId = Role.Manager, IsEnabled = true },
            new UserEntity { Id = 3, Email = "user@test.com", PasswordHash = "x", RoleId = Role.User, IsEnabled = true });

        await db.SaveChangesAsync(CancellationToken.None);
        db.ChangeTracker.Clear();
    }

    [Fact]
    public async Task Handle_ReturnsAllUsers_WhenCallerIsAdmin()
    {
        await using var db = TestDbContext.Create();
        await SeedUsers(db);

        var handler = new ListUserQueryHandler(db, new FakeCurrentUser { UserId = 1, IsAdmin = true });

        var result = await handler.Handle(new ListUserQuery(), CancellationToken.None);

        Assert.Equal(3, result.Total);
        Assert.Contains(result.Items, u => u.RoleId == (int)Role.Admin);
    }

    [Fact]
    public async Task Handle_ExcludesAdminUsers_WhenCallerIsManager()
    {
        await using var db = TestDbContext.Create();
        await SeedUsers(db);

        var handler = new ListUserQueryHandler(db, new FakeCurrentUser { UserId = 2, IsManager = true });

        var result = await handler.Handle(new ListUserQuery(), CancellationToken.None);

        Assert.Equal(2, result.Total);
        Assert.DoesNotContain(result.Items, u => u.RoleId == (int)Role.Admin);
    }
}
