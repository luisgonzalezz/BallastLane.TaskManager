using BallastLane.TaskManager.Domain.Entities;

namespace BallastLane.TaskManager.Application.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    Task AddAsync(User user, CancellationToken cancellationToken);
}
