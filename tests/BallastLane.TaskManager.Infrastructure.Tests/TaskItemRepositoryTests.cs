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

    [Fact]
    public async Task GetByIdAsync_WhenTaskBelongsToUser_ReturnsTask()
    {
        var connectionFactory = database.CreateConnectionFactory();
        var userRepository = new UserRepository(connectionFactory);
        var taskRepository = new TaskItemRepository(connectionFactory);
        var user = User.Create(UniqueEmail(), "hash");
        await userRepository.AddAsync(user, CancellationToken.None);
        var storedTask = TaskItem.Create(user.Id, "Find task", "Find by id", DateTime.UtcNow.AddDays(2));
        await taskRepository.AddAsync(storedTask, CancellationToken.None);

        var task = await taskRepository.GetByIdAsync(storedTask.Id, user.Id, CancellationToken.None);

        Assert.NotNull(task);
        Assert.Equal(storedTask.Id, task.Id);
        Assert.Equal("Find task", task.Title);
    }

    [Fact]
    public async Task UpdateAsync_PersistsTaskChanges()
    {
        var connectionFactory = database.CreateConnectionFactory();
        var userRepository = new UserRepository(connectionFactory);
        var taskRepository = new TaskItemRepository(connectionFactory);
        var user = User.Create(UniqueEmail(), "hash");
        await userRepository.AddAsync(user, CancellationToken.None);
        var storedTask = TaskItem.Create(user.Id, "Old title", "Old description", DateTime.UtcNow.AddDays(2));
        await taskRepository.AddAsync(storedTask, CancellationToken.None);
        var newDueDate = DateTime.UtcNow.AddDays(5);
        storedTask.UpdateDetails("New title", "New description", newDueDate, TaskItemStatus.Completed);

        await taskRepository.UpdateAsync(storedTask, CancellationToken.None);
        var persistedTask = await taskRepository.GetByIdAsync(storedTask.Id, user.Id, CancellationToken.None);

        Assert.NotNull(persistedTask);
        Assert.Equal("New title", persistedTask.Title);
        Assert.Equal("New description", persistedTask.Description);
        Assert.Equal(newDueDate, persistedTask.DueDate);
        Assert.Equal(TaskItemStatus.Completed, persistedTask.Status);
    }

    [Fact]
    public async Task DeleteAsync_RemovesOnlyRequestedUsersTask()
    {
        var connectionFactory = database.CreateConnectionFactory();
        var userRepository = new UserRepository(connectionFactory);
        var taskRepository = new TaskItemRepository(connectionFactory);
        var requestedUser = User.Create(UniqueEmail(), "hash");
        var otherUser = User.Create(UniqueEmail(), "hash");
        await userRepository.AddAsync(requestedUser, CancellationToken.None);
        await userRepository.AddAsync(otherUser, CancellationToken.None);
        var requestedTask = TaskItem.Create(requestedUser.Id, "Delete task", "Remove this", DateTime.UtcNow.AddDays(2));
        var otherTask = TaskItem.Create(otherUser.Id, "Keep task", "Keep this", DateTime.UtcNow.AddDays(2));
        await taskRepository.AddAsync(requestedTask, CancellationToken.None);
        await taskRepository.AddAsync(otherTask, CancellationToken.None);

        await taskRepository.DeleteAsync(requestedTask.Id, requestedUser.Id, CancellationToken.None);
        var deletedTask = await taskRepository.GetByIdAsync(requestedTask.Id, requestedUser.Id, CancellationToken.None);
        var remainingTasks = await taskRepository.GetByUserIdAsync(otherUser.Id, CancellationToken.None);

        Assert.Null(deletedTask);
        Assert.Single(remainingTasks);
    }

    private static string UniqueEmail()
    {
        return $"user-{Guid.NewGuid():N}@ballastlane.com";
    }
}
