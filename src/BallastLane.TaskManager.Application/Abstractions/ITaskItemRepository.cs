using BallastLane.TaskManager.Domain.Entities;

namespace BallastLane.TaskManager.Application.Abstractions;

public interface ITaskItemRepository
{
    Task AddAsync(TaskItem task, CancellationToken cancellationToken);

    Task<TaskItem?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);

    Task<IReadOnlyList<TaskItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task UpdateAsync(TaskItem task, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken);
}
