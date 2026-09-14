using FluentValidation.TestHelper;
using Stowaway.Application.Common.Exceptions;
using Stowaway.Application.Modules.Identity.Users.Commands.Update;
using Stowaway.Domain.Entities.Identity;
using Stowaway.Tests.Helpers;

namespace Stowaway.Tests.UnitTests;

public class UpdateUserCommandValidatorTests
{
    private static UpdateUserCommand ValidCommand(int id, string? email = "new@example.com") => new()
    {
        Id = id,
        Email = email,
        Password = null,
        FirstName = null,
        LastName = null,
        Role = null,
        IsEnabled = null
    };

    [Fact]
    public async Task Should_ThrowConflict_WhenEmailBelongsToAnotherUser()
    {
        await using var db = TestDbContext.Create();
        db.Users.Add(new UserEntity { Id = 1, Email = "taken@example.com", PasswordHash = "x", IsEnabled = true });
        db.Users.Add(new UserEntity { Id = 2, Email = "self@example.com", PasswordHash = "x", IsEnabled = true });
        await db.SaveChangesAsync(CancellationToken.None);

        var validator = new UpdateUserCommandValidator(db);

        await Assert.ThrowsAsync<StowawayConflictException>(() =>
            validator.TestValidateAsync(ValidCommand(id: 2, email: "taken@example.com")));
    }

    [Fact]
    public async Task Should_NotThrow_WhenEmailUnchangedForSameUser()
    {
        await using var db = TestDbContext.Create();
        db.Users.Add(new UserEntity { Id = 1, Email = "self@example.com", PasswordHash = "x", IsEnabled = true });
        await db.SaveChangesAsync(CancellationToken.None);

        var validator = new UpdateUserCommandValidator(db);

        var result = await validator.TestValidateAsync(ValidCommand(id: 1, email: "self@example.com"));

        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Should_NotThrow_WhenEmailIsNull()
    {
        await using var db = TestDbContext.Create();
        db.Users.Add(new UserEntity { Id = 1, Email = "self@example.com", PasswordHash = "x", IsEnabled = true });
        await db.SaveChangesAsync(CancellationToken.None);

        var validator = new UpdateUserCommandValidator(db);

        var result = await validator.TestValidateAsync(ValidCommand(id: 1, email: null));

        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }
}
