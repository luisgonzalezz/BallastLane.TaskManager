using BallastLane.TaskManager.Application.Abstractions;
using BallastLane.TaskManager.Application.Exceptions;
using BallastLane.TaskManager.Application.DTOs.Tasks;
using BallastLane.TaskManager.Domain.Entities;

namespace BallastLane.TaskManager.Application.Services;

public sealed class TaskItemService : ITaskItemService
{
    private readonly ITaskItemRepository taskItemRepository;

    public TaskItemService(ITaskItemRepository taskItemRepository)
    {
        this.taskItemRepository = taskItemRepository;
    }

    public async Task<TaskItemResponse> CreateAsync(
        Guid userId,
        CreateTaskItemRequest request,
        CancellationToken cancellationToken)
    {
        var task = TaskItem.Create(userId, request.Title, request.Description, request.DueDate);

        await taskItemRepository.AddAsync(task, cancellationToken);

        return MapToResponse(task);
    }

    public async Task<TaskItemResponse> GetByIdAsync(
        Guid userId,
        Guid id,
        CancellationToken cancellationToken)
    {
        var task = await GetRequiredTaskAsync(id, userId, cancellationToken);

        return MapToResponse(task);
    }

    public async Task<IReadOnlyList<TaskItemResponse>> GetForUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var tasks = await taskItemRepository.GetByUserIdAsync(userId, cancellationToken);

        return tasks.Select(MapToResponse).ToList();
    }

    public async Task<TaskItemResponse> UpdateAsync(
        Guid userId,
        Guid id,
        UpdateTaskItemRequest request,
        CancellationToken cancellationToken)
    {
        var task = await GetRequiredTaskAsync(id, userId, cancellationToken);
        task.UpdateDetails(request.Title, request.Description, request.DueDate, request.Status);

        await taskItemRepository.UpdateAsync(task, cancellationToken);

        return MapToResponse(task);
    }

    public async Task DeleteAsync(
        Guid userId,
        Guid id,
        CancellationToken cancellationToken)
    {
        _ = await GetRequiredTaskAsync(id, userId, cancellationToken);

        await taskItemRepository.DeleteAsync(id, userId, cancellationToken);
    }

    private async Task<TaskItem> GetRequiredTaskAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var task = await taskItemRepository.GetByIdAsync(id, userId, cancellationToken);

        return task ?? throw new ApplicationNotFoundException("Task was not found.");
    }

    private static TaskItemResponse MapToResponse(TaskItem task)
    {
        return new TaskItemResponse(
            task.Id,
            task.UserId,
            task.Title,
            task.Description,
            task.Status,
            task.DueDate,
            task.CreatedAt,
            task.UpdatedAt);
    }
}
