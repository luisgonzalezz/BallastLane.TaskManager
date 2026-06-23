using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BallastLane.TaskManager.Infrastructure.Data;

public sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is required.");
    }

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(connectionString);
    }
}
