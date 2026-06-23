using System.Data;
using BallastLane.TaskManager.Application.Abstractions;
using BallastLane.TaskManager.Domain.Entities;
using BallastLane.TaskManager.Domain.Enums;
using BallastLane.TaskManager.Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace BallastLane.TaskManager.Infrastructure.Repositories;

public sealed class TaskItemRepository : ITaskItemRepository
{
    private readonly ISqlConnectionFactory connectionFactory;

    public TaskItemRepository(ISqlConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public async Task AddAsync(TaskItem task, CancellationToken cancellationToken)
    {
        const string sql =
            """
            INSERT INTO dbo.Tasks (Id, UserId, Title, Description, Status, DueDate, CreatedAt, UpdatedAt)
            VALUES (@Id, @UserId, @Title, @Description, @Status, @DueDate, @CreatedAt, @UpdatedAt);
            """;

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", task.Id);
        command.Parameters.AddWithValue("@UserId", task.UserId);
        command.Parameters.AddWithValue("@Title", task.Title);
        command.Parameters.AddWithValue("@Description", task.Description);
        command.Parameters.AddWithValue("@Status", (int)task.Status);
        command.Parameters.Add("@DueDate", SqlDbType.DateTime2).Value = task.DueDate;
        command.Parameters.Add("@CreatedAt", SqlDbType.DateTime2).Value = task.CreatedAt;
        command.Parameters.Add("@UpdatedAt", SqlDbType.DateTime2).Value = task.UpdatedAt;

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        const string sql =
            """
            SELECT Id, UserId, Title, Description, Status, DueDate, CreatedAt, UpdatedAt
            FROM dbo.Tasks
            WHERE UserId = @UserId
            ORDER BY CreatedAt DESC;
            """;

        var tasks = new List<TaskItem>();
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@UserId", userId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            tasks.Add(MapTask(reader));
        }

        return tasks;
    }

    private static TaskItem MapTask(SqlDataReader reader)
    {
        return TaskItem.FromPersistence(
            reader.GetGuid(reader.GetOrdinal("Id")),
            reader.GetGuid(reader.GetOrdinal("UserId")),
            reader.GetString(reader.GetOrdinal("Title")),
            reader.GetString(reader.GetOrdinal("Description")),
            (TaskItemStatus)reader.GetInt32(reader.GetOrdinal("Status")),
            reader.GetDateTime(reader.GetOrdinal("DueDate")),
            reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            reader.GetDateTime(reader.GetOrdinal("UpdatedAt")));
    }
}
