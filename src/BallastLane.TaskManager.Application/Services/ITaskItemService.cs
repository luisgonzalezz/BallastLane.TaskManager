using BallastLane.TaskManager.Application.DTOs.Tasks;

namespace BallastLane.TaskManager.Application.Services;

public interface ITaskItemService
{
    Task<TaskItemResponse> CreateAsync(
        Guid userId,
        CreateTaskItemRequest request,
        CancellationToken cancellationToken);

    Task<TaskItemResponse> GetByIdAsync(
        Guid userId,
        Guid id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<TaskItemResponse>> GetForUserAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<TaskItemResponse> UpdateAsync(
        Guid userId,
        Guid id,
        UpdateTaskItemRequest request,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid userId,
        Guid id,
        CancellationToken cancellationToken);
}
