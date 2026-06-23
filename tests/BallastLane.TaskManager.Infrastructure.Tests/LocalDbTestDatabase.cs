using BallastLane.TaskManager.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BallastLane.TaskManager.Infrastructure.Tests;

public sealed class LocalDbTestDatabase : IAsyncLifetime
{
    private readonly string databaseName = $"BallastLaneTaskManager_Test_{Guid.NewGuid():N}";
    private readonly string masterConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";

    public string ConnectionString =>
        $"Server=(localdb)\\MSSQLLocalDB;Database={databaseName};Trusted_Connection=True;TrustServerCertificate=True;";

    public ISqlConnectionFactory CreateConnectionFactory()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = ConnectionString
            })
            .Build();

        return new SqlConnectionFactory(configuration);
    }

    public async Task InitializeAsync()
    {
        await ExecuteMasterAsync($"CREATE DATABASE [{databaseName}];");
        await ExecuteDatabaseAsync(
            """
            CREATE TABLE dbo.Users
            (
                Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
                Email NVARCHAR(256) NOT NULL,
                PasswordHash NVARCHAR(512) NOT NULL,
                CreatedAt DATETIME2(7) NOT NULL
            );

            CREATE UNIQUE INDEX UX_Users_Email ON dbo.Users (Email);

            CREATE TABLE dbo.Tasks
            (
                Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Tasks PRIMARY KEY,
                UserId UNIQUEIDENTIFIER NOT NULL,
                Title NVARCHAR(200) NOT NULL,
                Description NVARCHAR(1000) NOT NULL,
                Status INT NOT NULL,
                DueDate DATETIME2(7) NOT NULL,
                CreatedAt DATETIME2(7) NOT NULL,
                UpdatedAt DATETIME2(7) NOT NULL,
                CONSTRAINT FK_Tasks_Users_UserId FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
                CONSTRAINT CK_Tasks_Status CHECK (Status IN (1, 2, 3))
            );

            CREATE INDEX IX_Tasks_UserId ON dbo.Tasks (UserId);
            """);
    }

    public async Task DisposeAsync()
    {
        SqlConnection.ClearAllPools();

        await ExecuteMasterAsync(
            $"""
            IF DB_ID(N'{databaseName}') IS NOT NULL
            BEGIN
                ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{databaseName}];
            END;
            """);
    }

    private async Task ExecuteMasterAsync(string sql)
    {
        await using var connection = new SqlConnection(masterConnectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    private async Task ExecuteDatabaseAsync(string sql)
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }
}
