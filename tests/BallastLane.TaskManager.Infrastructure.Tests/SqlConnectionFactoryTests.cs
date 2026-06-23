using BallastLane.TaskManager.Infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace BallastLane.TaskManager.Infrastructure.Tests;

public sealed class SqlConnectionFactoryTests
{
    [Fact]
    public void CreateConnection_UsesDefaultConnectionString()
    {
        const string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=BallastLaneTaskManager;Trusted_Connection=True;";
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = connectionString
            })
            .Build();
        var factory = new SqlConnectionFactory(configuration);

        using var connection = factory.CreateConnection();

        Assert.Equal(connectionString, connection.ConnectionString);
    }
}
