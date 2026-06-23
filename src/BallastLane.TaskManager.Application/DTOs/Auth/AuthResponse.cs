namespace BallastLane.TaskManager.Application.DTOs.Auth;

public sealed record AuthResponse(Guid UserId, string Email, string Token);
