using BallastLane.TaskManager.Domain.Entities;
using BallastLane.TaskManager.Domain.Exceptions;

namespace BallastLane.TaskManager.Domain.Tests;

public sealed class UserTests
{
    [Fact]
    public void Create_WithValidValues_CreatesUser()
    {
        var user = User.Create("demo@ballastlane.com", "hashed-password");

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("demo@ballastlane.com", user.Email);
        Assert.Equal("hashed-password", user.PasswordHash);
        Assert.True(user.CreatedAt <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("missing-domain@")]
    public void Create_WithInvalidEmail_ThrowsDomainValidationException(string email)
    {
        var exception = Assert.Throws<DomainValidationException>(
            () => User.Create(email, "hashed-password"));

        Assert.Equal("A valid email is required.", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankPasswordHash_ThrowsDomainValidationException(string passwordHash)
    {
        var exception = Assert.Throws<DomainValidationException>(
            () => User.Create("demo@ballastlane.com", passwordHash));

        Assert.Equal("Password hash is required.", exception.Message);
    }

    [Fact]
    public void FromPersistence_WithStoredValues_RehydratesUser()
    {
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow.AddDays(-1);

        var user = User.FromPersistence(id, "demo@ballastlane.com", "stored-hash", createdAt);

        Assert.Equal(id, user.Id);
        Assert.Equal("demo@ballastlane.com", user.Email);
        Assert.Equal("stored-hash", user.PasswordHash);
        Assert.Equal(createdAt, user.CreatedAt);
    }
}
