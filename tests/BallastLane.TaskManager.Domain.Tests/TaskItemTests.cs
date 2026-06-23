using BallastLane.TaskManager.Domain.Entities;
using BallastLane.TaskManager.Domain.Enums;
using BallastLane.TaskManager.Domain.Exceptions;

namespace BallastLane.TaskManager.Domain.Tests;

public sealed class TaskItemTests
{
    [Fact]
    public void Create_WithValidValues_CreatesPendingTaskForUser()
    {
        var userId = Guid.NewGuid();
        var dueDate = DateTime.UtcNow.AddDays(1);

        var task = TaskItem.Create(userId, "Prepare interview", "Review Clean Architecture", dueDate);

        Assert.NotEqual(Guid.Empty, task.Id);
        Assert.Equal(userId, task.UserId);
        Assert.Equal("Prepare interview", task.Title);
        Assert.Equal("Review Clean Architecture", task.Description);
        Assert.Equal(TaskItemStatus.Pending, task.Status);
        Assert.Equal(dueDate, task.DueDate);
        Assert.True(task.CreatedAt <= DateTime.UtcNow);
        Assert.True(task.UpdatedAt <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankTitle_ThrowsDomainValidationException(string title)
    {
        var exception = Assert.Throws<DomainValidationException>(
            () => TaskItem.Create(Guid.NewGuid(), title, "Description", DateTime.UtcNow.AddDays(1)));

        Assert.Equal("Task title is required.", exception.Message);
    }

    [Fact]
    public void Create_WithEmptyUserId_ThrowsDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(
            () => TaskItem.Create(Guid.Empty, "Title", "Description", DateTime.UtcNow.AddDays(1)));

        Assert.Equal("Task must belong to a user.", exception.Message);
    }

    [Fact]
    public void Create_WithDefaultDueDate_ThrowsDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(
            () => TaskItem.Create(Guid.NewGuid(), "Title", "Description", default));

        Assert.Equal("Due date is required.", exception.Message);
    }

    [Fact]
    public void Create_WithNullDescription_CreatesTaskWithEmptyDescription()
    {
        var task = TaskItem.Create(Guid.NewGuid(), "Title", null!, DateTime.UtcNow.AddDays(1));

        Assert.Equal(string.Empty, task.Description);
    }

    [Fact]
    public void MarkInProgress_UpdatesStatusAndTimestamp()
    {
        var task = TaskItem.Create(Guid.NewGuid(), "Title", "Description", DateTime.UtcNow.AddDays(1));
        var originalUpdatedAt = task.UpdatedAt;

        task.MarkInProgress();

        Assert.Equal(TaskItemStatus.InProgress, task.Status);
        Assert.True(task.UpdatedAt >= originalUpdatedAt);
    }

    [Fact]
    public void MarkCompleted_UpdatesStatusAndTimestamp()
    {
        var task = TaskItem.Create(Guid.NewGuid(), "Title", "Description", DateTime.UtcNow.AddDays(1));
        var originalUpdatedAt = task.UpdatedAt;

        task.MarkCompleted();

        Assert.Equal(TaskItemStatus.Completed, task.Status);
        Assert.True(task.UpdatedAt >= originalUpdatedAt);
    }

    [Fact]
    public void Rename_WithBlankTitle_ThrowsDomainValidationException()
    {
        var task = TaskItem.Create(Guid.NewGuid(), "Title", "Description", DateTime.UtcNow.AddDays(1));

        var exception = Assert.Throws<DomainValidationException>(() => task.Rename(" "));

        Assert.Equal("Task title is required.", exception.Message);
    }
}
