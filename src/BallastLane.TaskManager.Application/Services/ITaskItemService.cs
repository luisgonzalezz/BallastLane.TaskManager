using BallastLane.TaskManager.Application.DTOs.Tasks;

namespace BallastLane.TaskManager.Application.Services;

public interface ITaskItemService
{
    Task<TaskItemResponse> CreateAsync(
        Guid userId,
        CreateTaskItemRequest request,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<TaskItemResponse>> GetForUserAsync(
        Guid userId,
        CancellationToken cancellationToken);
}
