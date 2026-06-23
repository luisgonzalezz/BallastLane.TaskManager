using System.Data;
using BallastLane.TaskManager.Application.Abstractions;
using BallastLane.TaskManager.Domain.Entities;
using BallastLane.TaskManager.Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace BallastLane.TaskManager.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly ISqlConnectionFactory connectionFactory;

    public UserRepository(ISqlConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        const string sql =
            """
            SELECT TOP (1) Id, Email, PasswordHash, CreatedAt
            FROM dbo.Users
            WHERE Email = @Email;
            """;

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Email", email);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return MapUser(reader);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        const string sql =
            """
            INSERT INTO dbo.Users (Id, Email, PasswordHash, CreatedAt)
            VALUES (@Id, @Email, @PasswordHash, @CreatedAt);
            """;

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", user.Id);
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
        command.Parameters.Add("@CreatedAt", SqlDbType.DateTime2).Value = user.CreatedAt;

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static User MapUser(SqlDataReader reader)
    {
        return User.FromPersistence(
            reader.GetGuid(reader.GetOrdinal("Id")),
            reader.GetString(reader.GetOrdinal("Email")),
            reader.GetString(reader.GetOrdinal("PasswordHash")),
            reader.GetDateTime(reader.GetOrdinal("CreatedAt")));
    }
}
