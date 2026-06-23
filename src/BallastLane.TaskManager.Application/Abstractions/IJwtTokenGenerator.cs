using BallastLane.TaskManager.Domain.Entities;

namespace BallastLane.TaskManager.Application.Abstractions;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
