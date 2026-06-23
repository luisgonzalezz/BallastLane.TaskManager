using BallastLane.TaskManager.Domain.Entities;
using BallastLane.TaskManager.Infrastructure.Repositories;

namespace BallastLane.TaskManager.Infrastructure.Tests;

public sealed class UserRepositoryTests : IClassFixture<LocalDbTestDatabase>
{
    private readonly LocalDbTestDatabase database;

    public UserRepositoryTests(LocalDbTestDatabase database)
    {
        this.database = database;
    }

    [Fact]
    public async Task AddAsync_ThenGetByEmailAsync_ReturnsPersistedUser()
    {
        var repository = new UserRepository(database.CreateConnectionFactory());
        var createdAt = TrimToSqlPrecision(DateTime.UtcNow);
        var user = User.FromPersistence(Guid.NewGuid(), "demo@ballastlane.com", "stored-hash", createdAt);

        await repository.AddAsync(user, CancellationToken.None);
        var persistedUser = await repository.GetByEmailAsync("demo@ballastlane.com", CancellationToken.None);

        Assert.NotNull(persistedUser);
        Assert.Equal(user.Id, persistedUser.Id);
        Assert.Equal(user.Email, persistedUser.Email);
        Assert.Equal(user.PasswordHash, persistedUser.PasswordHash);
        Assert.Equal(user.CreatedAt, persistedUser.CreatedAt);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenEmailDoesNotExist_ReturnsNull()
    {
        var repository = new UserRepository(database.CreateConnectionFactory());

        var persistedUser = await repository.GetByEmailAsync("missing@ballastlane.com", CancellationToken.None);

        Assert.Null(persistedUser);
    }

    private static DateTime TrimToSqlPrecision(DateTime value)
    {
        return new DateTime(value.Ticks, DateTimeKind.Utc);
    }
}
