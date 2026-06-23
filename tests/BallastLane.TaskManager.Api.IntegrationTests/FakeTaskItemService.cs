using BallastLane.TaskManager.Application.DTOs.Tasks;
using BallastLane.TaskManager.Application.Services;
using BallastLane.TaskManager.Domain.Enums;

namespace BallastLane.TaskManager.Api.IntegrationTests;

public sealed class FakeTaskItemService : ITaskItemService
{
    public Task<TaskItemResponse> CreateAsync(
        Guid userId,
        CreateTaskItemRequest request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new TaskItemResponse(
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            userId,
            request.Title,
            request.Description ?? string.Empty,
            TaskItemStatus.Pending,
            request.DueDate,
            DateTime.UtcNow,
            DateTime.UtcNow));
    }

    public Task<TaskItemResponse> GetByIdAsync(
        Guid userId,
        Guid id,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(CreateResponse(id, userId, "Read task", "Single task", TaskItemStatus.InProgress));
    }

    public Task<IReadOnlyList<TaskItemResponse>> GetForUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<TaskItemResponse> tasks =
        [
            CreateResponse(Guid.Parse("33333333-3333-3333-3333-333333333333"), userId, "List task", "Listed task", TaskItemStatus.Pending)
        ];

        return Task.FromResult(tasks);
    }

    public Task<TaskItemResponse> UpdateAsync(
        Guid userId,
        Guid id,
        UpdateTaskItemRequest request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new TaskItemResponse(
            id,
            userId,
            request.Title,
            request.Description ?? string.Empty,
            request.Status,
            request.DueDate,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow));
    }

    public Task DeleteAsync(
        Guid userId,
        Guid id,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static TaskItemResponse CreateResponse(
        Guid id,
        Guid userId,
        string title,
        string description,
        TaskItemStatus status)
    {
        return new TaskItemResponse(
            id,
            userId,
            title,
            description,
            status,
            DateTime.UtcNow.AddDays(2),
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow);
    }
}
