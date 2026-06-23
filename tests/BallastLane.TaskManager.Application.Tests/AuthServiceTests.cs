using BallastLane.TaskManager.Application.Abstractions;
using BallastLane.TaskManager.Application.DTOs.Auth;
using BallastLane.TaskManager.Application.Exceptions;
using BallastLane.TaskManager.Application.Services;
using BallastLane.TaskManager.Domain.Entities;

namespace BallastLane.TaskManager.Application.Tests;

public sealed class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_WithNewEmail_CreatesUserAndReturnsToken()
    {
        var users = new FakeUserRepository();
        var passwordHasher = new FakePasswordHasher();
        var tokenGenerator = new FakeJwtTokenGenerator();
        var service = new AuthService(users, passwordHasher, tokenGenerator);

        var response = await service.RegisterAsync(
            new RegisterUserRequest("demo@ballastlane.com", "Password123!"),
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, response.UserId);
        Assert.Equal("demo@ballastlane.com", response.Email);
        Assert.Equal("token-for-demo@ballastlane.com", response.Token);
        Assert.Single(users.Users);
        Assert.Equal("hashed:Password123!", users.Users[0].PasswordHash);
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ThrowsApplicationValidationException()
    {
        var users = new FakeUserRepository();
        await users.AddAsync(User.Create("demo@ballastlane.com", "existing-hash"), CancellationToken.None);
        var service = new AuthService(users, new FakePasswordHasher(), new FakeJwtTokenGenerator());

        var exception = await Assert.ThrowsAsync<ApplicationValidationException>(
            () => service.RegisterAsync(
                new RegisterUserRequest("demo@ballastlane.com", "Password123!"),
                CancellationToken.None));

        Assert.Equal("Email is already registered.", exception.Message);
    }

    [Fact]
    public async Task RegisterAsync_WithBlankPassword_ThrowsApplicationValidationException()
    {
        var service = new AuthService(
            new FakeUserRepository(),
            new FakePasswordHasher(),
            new FakeJwtTokenGenerator());

        var exception = await Assert.ThrowsAsync<ApplicationValidationException>(
            () => service.RegisterAsync(
                new RegisterUserRequest("demo@ballastlane.com", " "),
                CancellationToken.None));

        Assert.Equal("Password is required.", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsToken()
    {
        var users = new FakeUserRepository();
        await users.AddAsync(User.Create("demo@ballastlane.com", "hashed:Password123!"), CancellationToken.None);
        var service = new AuthService(users, new FakePasswordHasher(), new FakeJwtTokenGenerator());

        var response = await service.LoginAsync(
            new LoginRequest("demo@ballastlane.com", "Password123!"),
            CancellationToken.None);

        Assert.Equal("demo@ballastlane.com", response.Email);
        Assert.Equal("token-for-demo@ballastlane.com", response.Token);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsApplicationValidationException()
    {
        var users = new FakeUserRepository();
        await users.AddAsync(User.Create("demo@ballastlane.com", "hashed:Password123!"), CancellationToken.None);
        var service = new AuthService(users, new FakePasswordHasher(), new FakeJwtTokenGenerator());

        var exception = await Assert.ThrowsAsync<ApplicationValidationException>(
            () => service.LoginAsync(
                new LoginRequest("demo@ballastlane.com", "wrong-password"),
                CancellationToken.None));

        Assert.Equal("Invalid email or password.", exception.Message);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public List<User> Users { get; } = [];

        public Task AddAsync(User user, CancellationToken cancellationToken)
        {
            Users.Add(user);
            return Task.CompletedTask;
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var user = Users.SingleOrDefault(
                candidate => string.Equals(candidate.Email, email, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(user);
        }
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return $"hashed:{password}";
        }

        public bool Verify(string password, string passwordHash)
        {
            return passwordHash == Hash(password);
        }
    }

    private sealed class FakeJwtTokenGenerator : IJwtTokenGenerator
    {
        public string GenerateToken(User user)
        {
            return $"token-for-{user.Email}";
        }
    }
}
