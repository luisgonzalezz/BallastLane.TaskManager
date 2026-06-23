using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BallastLane.TaskManager.Domain.Entities;
using BallastLane.TaskManager.Infrastructure.Security;
using Microsoft.Extensions.Configuration;

namespace BallastLane.TaskManager.Infrastructure.Tests;

public sealed class JwtTokenGeneratorTests
{
    [Fact]
    public void GenerateToken_WithValidUser_IncludesUserClaims()
    {
        var generator = new JwtTokenGenerator(CreateConfiguration());
        var user = User.Create("demo@ballastlane.com", "hashed-password");

        var token = generator.GenerateToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal("BallastLane.TaskManager", jwt.Issuer);
        Assert.Equal("BallastLane.TaskManager.Client", jwt.Audiences.Single());
        Assert.Contains(jwt.Claims, claim => claim.Type == ClaimTypes.NameIdentifier && claim.Value == user.Id.ToString());
        Assert.Contains(jwt.Claims, claim => claim.Type == ClaimTypes.Email && claim.Value == user.Email);
    }

    private static IConfiguration CreateConfiguration()
    {
        var values = new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "BallastLane.TaskManager",
            ["Jwt:Audience"] = "BallastLane.TaskManager.Client",
            ["Jwt:SigningKey"] = "this-is-a-demo-signing-key-with-32-chars",
            ["Jwt:ExpirationMinutes"] = "60"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }
}
