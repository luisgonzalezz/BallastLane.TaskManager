using BallastLane.TaskManager.Application.Abstractions;
using BallastLane.TaskManager.Application.Exceptions;
using BallastLane.TaskManager.Application.DTOs.Tasks;
using BallastLane.TaskManager.Application.Services;
using BallastLane.TaskManager.Domain.Entities;
using BallastLane.TaskManager.Domain.Enums;

namespace BallastLane.TaskManager.Application.Tests;

public sealed class TaskItemServiceTests
{
    [Fact]
    public async Task CreateAsync_WithValidRequest_CreatesTaskForUser()
    {
        var tasks = new FakeTaskItemRepository();
        var service = new TaskItemService(tasks);
        var userId = Guid.NewGuid();
        var dueDate = DateTime.UtcNow.AddDays(2);

        var response = await service.CreateAsync(
            userId,
            new CreateTaskItemRequest("Write README", "Document setup", dueDate),
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(userId, response.UserId);
        Assert.Equal("Write README", response.Title);
        Assert.Equal("Document setup", response.Description);
        Assert.Equal(TaskItemStatus.Pending, response.Status);
        Assert.Equal(dueDate, response.DueDate);
        Assert.Single(tasks.Tasks);
    }

    [Fact]
    public async Task GetForUserAsync_ReturnsOnlyTasksForRequestedUser()
    {
        var requestedUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var tasks = new FakeTaskItemRepository();
        await tasks.AddAsync(TaskItem.Create(requestedUserId, "First", "One", DateTime.UtcNow.AddDays(1)), CancellationToken.None);
        await tasks.AddAsync(TaskItem.Create(otherUserId, "Second", "Two", DateTime.UtcNow.AddDays(1)), CancellationToken.None);
        var service = new TaskItemService(tasks);

        var response = await service.GetForUserAsync(requestedUserId, CancellationToken.None);

        var task = Assert.Single(response);
        Assert.Equal("First", task.Title);
        Assert.Equal(requestedUserId, task.UserId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTaskBelongsToUser_ReturnsTask()
    {
        var userId = Guid.NewGuid();
        var storedTask = TaskItem.Create(userId, "Read task", "Existing", DateTime.UtcNow.AddDays(1));
        var tasks = new FakeTaskItemRepository();
        await tasks.AddAsync(storedTask, CancellationToken.None);
        var service = new TaskItemService(tasks);

        var response = await service.GetByIdAsync(userId, storedTask.Id, CancellationToken.None);

        Assert.Equal(storedTask.Id, response.Id);
        Assert.Equal("Read task", response.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTaskDoesNotBelongToUser_ThrowsApplicationNotFoundException()
    {
        var storedTask = TaskItem.Create(Guid.NewGuid(), "Read task", "Existing", DateTime.UtcNow.AddDays(1));
        var tasks = new FakeTaskItemRepository();
        await tasks.AddAsync(storedTask, CancellationToken.None);
        var service = new TaskItemService(tasks);

        var exception = await Assert.ThrowsAsync<ApplicationNotFoundException>(
            () => service.GetByIdAsync(Guid.NewGuid(), storedTask.Id, CancellationToken.None));

        Assert.Equal("Task was not found.", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenTaskBelongsToUser_UpdatesTask()
    {
        var userId = Guid.NewGuid();
        var storedTask = TaskItem.Create(userId, "Old title", "Old description", DateTime.UtcNow.AddDays(1));
        var tasks = new FakeTaskItemRepository();
        await tasks.AddAsync(storedTask, CancellationToken.None);
        var service = new TaskItemService(tasks);
        var newDueDate = DateTime.UtcNow.AddDays(4);

        var response = await service.UpdateAsync(
            userId,
            storedTask.Id,
            new UpdateTaskItemRequest("New title", "New description", newDueDate, TaskItemStatus.Completed),
            CancellationToken.None);

        Assert.Equal(storedTask.Id, response.Id);
        Assert.Equal("New title", response.Title);
        Assert.Equal("New description", response.Description);
        Assert.Equal(newDueDate, response.DueDate);
        Assert.Equal(TaskItemStatus.Completed, response.Status);
        Assert.Equal(storedTask.Id, tasks.UpdatedTaskId);
    }

    [Fact]
    public async Task DeleteAsync_WhenTaskBelongsToUser_DeletesTask()
    {
        var userId = Guid.NewGuid();
        var storedTask = TaskItem.Create(userId, "Delete me", "Remove", DateTime.UtcNow.AddDays(1));
        var tasks = new FakeTaskItemRepository();
        await tasks.AddAsync(storedTask, CancellationToken.None);
        var service = new TaskItemService(tasks);

        await service.DeleteAsync(userId, storedTask.Id, CancellationToken.None);

        Assert.Equal(storedTask.Id, tasks.DeletedTaskId);
        Assert.Empty(tasks.Tasks);
    }

    private sealed class FakeTaskItemRepository : ITaskItemRepository
    {
        public List<TaskItem> Tasks { get; } = [];

        public Guid? UpdatedTaskId { get; private set; }

        public Guid? DeletedTaskId { get; private set; }

        public Task AddAsync(TaskItem task, CancellationToken cancellationToken)
        {
            Tasks.Add(task);
            return Task.CompletedTask;
        }

        public Task<TaskItem?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var task = Tasks.SingleOrDefault(candidate => candidate.Id == id && candidate.UserId == userId);

            return Task.FromResult(task);
        }

        public Task<IReadOnlyList<TaskItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            IReadOnlyList<TaskItem> tasks = Tasks
                .Where(task => task.UserId == userId)
                .ToList();

            return Task.FromResult(tasks);
        }

        public Task UpdateAsync(TaskItem task, CancellationToken cancellationToken)
        {
            UpdatedTaskId = task.Id;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            DeletedTaskId = id;
            Tasks.RemoveAll(task => task.Id == id && task.UserId == userId);
            return Task.CompletedTask;
        }
    }
}
