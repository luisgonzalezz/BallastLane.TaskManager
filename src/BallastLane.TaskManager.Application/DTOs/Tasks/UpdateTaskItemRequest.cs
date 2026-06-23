using BallastLane.TaskManager.Domain.Enums;

namespace BallastLane.TaskManager.Application.DTOs.Tasks;

public sealed record UpdateTaskItemRequest(
    string Title,
    string? Description,
    DateTime DueDate,
    TaskItemStatus Status);
