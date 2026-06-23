using BallastLane.TaskManager.Infrastructure.Security;

namespace BallastLane.TaskManager.Infrastructure.Tests;

public sealed class PasswordHasherTests
{
    [Fact]
    public void Hash_WithPlainPassword_ReturnsDifferentValueThanPassword()
    {
        var hasher = new PasswordHasher();

        var hash = hasher.Hash("Password123!");

        Assert.NotEqual("Password123!", hash);
        Assert.Contains('.', hash);
    }

    [Fact]
    public void Verify_WithMatchingPassword_ReturnsTrue()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.Hash("Password123!");

        var isValid = hasher.Verify("Password123!", hash);

        Assert.True(isValid);
    }

    [Fact]
    public void Verify_WithDifferentPassword_ReturnsFalse()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.Hash("Password123!");

        var isValid = hasher.Verify("wrong-password", hash);

        Assert.False(isValid);
    }
}
