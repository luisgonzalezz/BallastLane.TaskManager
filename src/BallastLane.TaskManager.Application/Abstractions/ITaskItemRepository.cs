using BallastLane.TaskManager.Domain.Entities;

namespace BallastLane.TaskManager.Application.Abstractions;

public interface ITaskItemRepository
{
    Task AddAsync(TaskItem task, CancellationToken cancellationToken);

    Task<IReadOnlyList<TaskItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}
