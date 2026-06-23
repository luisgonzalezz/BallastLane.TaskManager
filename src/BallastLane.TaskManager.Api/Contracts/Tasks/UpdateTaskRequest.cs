using BallastLane.TaskManager.Domain.Enums;

namespace BallastLane.TaskManager.Api.Contracts.Tasks;

public sealed record UpdateTaskRequest(
    string Title,
    string? Description,
    DateTime DueDate,
    TaskItemStatus Status);
