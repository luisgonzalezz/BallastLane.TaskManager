using BallastLane.TaskManager.Domain.Entities;
using BallastLane.TaskManager.Infrastructure.Security;
using Microsoft.Extensions.Configuration;

namespace BallastLane.TaskManager.Api.IntegrationTests;

public static class TestJwtTokenFactory
{
    public static string CreateToken(Guid userId, string email = "tasks-user@ballastlane.com")
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "BallastLane.TaskManager",
                ["Jwt:Audience"] = "BallastLane.TaskManager.Client",
                ["Jwt:SigningKey"] = "replace-this-demo-signing-key-before-production",
                ["Jwt:ExpirationMinutes"] = "60"
            })
            .Build();
        var generator = new JwtTokenGenerator(configuration);
        var user = User.FromPersistence(userId, email, "hash", DateTime.UtcNow);

        return generator.GenerateToken(user);
    }
}
