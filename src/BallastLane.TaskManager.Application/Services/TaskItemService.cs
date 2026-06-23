using BallastLane.TaskManager.Application.Abstractions;
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

    public async Task<IReadOnlyList<TaskItemResponse>> GetForUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var tasks = await taskItemRepository.GetByUserIdAsync(userId, cancellationToken);

        return tasks.Select(MapToResponse).ToList();
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
