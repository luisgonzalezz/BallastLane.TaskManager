namespace BallastLane.TaskManager.Application.DTOs.Tasks;

public sealed record CreateTaskItemRequest(string Title, string? Description, DateTime DueDate);
