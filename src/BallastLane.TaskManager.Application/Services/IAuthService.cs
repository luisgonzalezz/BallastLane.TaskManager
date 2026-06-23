using BallastLane.TaskManager.Application.DTOs.Auth;

namespace BallastLane.TaskManager.Application.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken);

    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}
