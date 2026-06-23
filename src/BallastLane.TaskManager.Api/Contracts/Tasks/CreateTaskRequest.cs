namespace BallastLane.TaskManager.Api.Contracts.Tasks;

public sealed record CreateTaskRequest(string Title, string? Description, DateTime DueDate);
