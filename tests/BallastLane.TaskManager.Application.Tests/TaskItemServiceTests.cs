using BallastLane.TaskManager.Application.Abstractions;
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

    private sealed class FakeTaskItemRepository : ITaskItemRepository
    {
        public List<TaskItem> Tasks { get; } = [];

        public Task AddAsync(TaskItem task, CancellationToken cancellationToken)
        {
            Tasks.Add(task);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<TaskItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            IReadOnlyList<TaskItem> tasks = Tasks
                .Where(task => task.UserId == userId)
                .ToList();

            return Task.FromResult(tasks);
        }
    }
}
