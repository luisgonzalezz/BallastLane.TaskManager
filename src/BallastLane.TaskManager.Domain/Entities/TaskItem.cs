using BallastLane.TaskManager.Domain.Enums;
using BallastLane.TaskManager.Domain.Exceptions;

namespace BallastLane.TaskManager.Domain.Entities;

public sealed class TaskItem
{
    private TaskItem(
        Guid id,
        Guid userId,
        string title,
        string description,
        TaskItemStatus status,
        DateTime dueDate,
        DateTime createdAt,
        DateTime updatedAt)
    {
        Id = id;
        UserId = userId;
        Title = title;
        Description = description;
        Status = status;
        DueDate = dueDate;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid Id { get; }

    public Guid UserId { get; }

    public string Title { get; private set; }

    public string Description { get; private set; }

    public TaskItemStatus Status { get; private set; }

    public DateTime DueDate { get; private set; }

    public DateTime CreatedAt { get; }

    public DateTime UpdatedAt { get; private set; }

    public static TaskItem Create(Guid userId, string title, string? description, DateTime dueDate)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainValidationException("Task must belong to a user.");
        }

        ValidateTitle(title);
        ValidateDueDate(dueDate);

        var now = DateTime.UtcNow;

        return new TaskItem(
            Guid.NewGuid(),
            userId,
            title.Trim(),
            description?.Trim() ?? string.Empty,
            TaskItemStatus.Pending,
            dueDate,
            now,
            now);
    }

    public static TaskItem FromPersistence(
        Guid id,
        Guid userId,
        string title,
        string? description,
        TaskItemStatus status,
        DateTime dueDate,
        DateTime createdAt,
        DateTime updatedAt)
    {
        if (id == Guid.Empty)
        {
            throw new DomainValidationException("Task id is required.");
        }

        if (userId == Guid.Empty)
        {
            throw new DomainValidationException("Task must belong to a user.");
        }

        ValidateTitle(title);
        ValidateDueDate(dueDate);

        if (!Enum.IsDefined(status))
        {
            throw new DomainValidationException("Task status is invalid.");
        }

        return new TaskItem(
            id,
            userId,
            title.Trim(),
            description?.Trim() ?? string.Empty,
            status,
            dueDate,
            createdAt,
            updatedAt);
    }

    public void Rename(string title)
    {
        ValidateTitle(title);

        Title = title.Trim();
        Touch();
    }

    public void UpdateDetails(
        string title,
        string? description,
        DateTime dueDate,
        TaskItemStatus status)
    {
        ValidateTitle(title);
        ValidateDueDate(dueDate);

        if (!Enum.IsDefined(status))
        {
            throw new DomainValidationException("Task status is invalid.");
        }

        Title = title.Trim();
        Description = description?.Trim() ?? string.Empty;
        DueDate = dueDate;
        Status = status;
        Touch();
    }

    public void MarkInProgress()
    {
        Status = TaskItemStatus.InProgress;
        Touch();
    }

    public void MarkCompleted()
    {
        Status = TaskItemStatus.Completed;
        Touch();
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainValidationException("Task title is required.");
        }
    }

    private static void ValidateDueDate(DateTime dueDate)
    {
        if (dueDate == default)
        {
            throw new DomainValidationException("Due date is required.");
        }
    }

    private void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
