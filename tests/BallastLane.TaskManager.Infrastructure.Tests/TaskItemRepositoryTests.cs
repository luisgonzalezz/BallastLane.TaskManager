using BallastLane.TaskManager.Domain.Entities;
using BallastLane.TaskManager.Domain.Enums;
using BallastLane.TaskManager.Infrastructure.Repositories;

namespace BallastLane.TaskManager.Infrastructure.Tests;

public sealed class TaskItemRepositoryTests : IClassFixture<LocalDbTestDatabase>
{
    private readonly LocalDbTestDatabase database;

    public TaskItemRepositoryTests(LocalDbTestDatabase database)
    {
        this.database = database;
    }

    [Fact]
    public async Task AddAsync_ThenGetByUserIdAsync_ReturnsOnlyTasksForUser()
    {
        var connectionFactory = database.CreateConnectionFactory();
        var userRepository = new UserRepository(connectionFactory);
        var taskRepository = new TaskItemRepository(connectionFactory);
        var requestedUser = User.Create("tasks-demo@ballastlane.com", "hash");
        var otherUser = User.Create("other-demo@ballastlane.com", "hash");
        await userRepository.AddAsync(requestedUser, CancellationToken.None);
        await userRepository.AddAsync(otherUser, CancellationToken.None);
        var requestedTask = TaskItem.FromPersistence(
            Guid.NewGuid(),
            requestedUser.Id,
            "Requested task",
            "Visible task",
            TaskItemStatus.InProgress,
            DateTime.UtcNow.AddDays(2),
            DateTime.UtcNow.AddDays(-2),
            DateTime.UtcNow.AddDays(-1));
        var otherTask = TaskItem.Create(otherUser.Id, "Other task", "Hidden task", DateTime.UtcNow.AddDays(3));

        await taskRepository.AddAsync(requestedTask, CancellationToken.None);
        await taskRepository.AddAsync(otherTask, CancellationToken.None);
        var tasks = await taskRepository.GetByUserIdAsync(requestedUser.Id, CancellationToken.None);

        var task = Assert.Single(tasks);
        Assert.Equal(requestedTask.Id, task.Id);
        Assert.Equal(requestedUser.Id, task.UserId);
        Assert.Equal("Requested task", task.Title);
        Assert.Equal("Visible task", task.Description);
        Assert.Equal(TaskItemStatus.InProgress, task.Status);
        Assert.Equal(requestedTask.DueDate, task.DueDate);
        Assert.Equal(requestedTask.CreatedAt, task.CreatedAt);
        Assert.Equal(requestedTask.UpdatedAt, task.UpdatedAt);
    }
}
