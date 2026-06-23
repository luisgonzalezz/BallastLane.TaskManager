using BallastLane.TaskManager.Domain.Enums;

namespace BallastLane.TaskManager.Application.DTOs.Tasks;

public sealed record TaskItemResponse(
    Guid Id,
    Guid UserId,
    string Title,
    string Description,
    TaskItemStatus Status,
    DateTime DueDate,
    DateTime CreatedAt,
    DateTime UpdatedAt);
