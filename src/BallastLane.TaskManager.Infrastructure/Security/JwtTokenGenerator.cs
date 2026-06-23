using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BallastLane.TaskManager.Application.Abstractions;
using BallastLane.TaskManager.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BallastLane.TaskManager.Infrastructure.Security;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly string issuer;
    private readonly string audience;
    private readonly string signingKey;
    private readonly int expirationMinutes;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        issuer = GetRequiredValue(configuration, "Jwt:Issuer");
        audience = GetRequiredValue(configuration, "Jwt:Audience");
        signingKey = GetRequiredValue(configuration, "Jwt:SigningKey");
        expirationMinutes = int.TryParse(configuration["Jwt:ExpirationMinutes"], out var configuredExpirationMinutes)
            ? configuredExpirationMinutes
            : 60;
    }

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GetRequiredValue(IConfiguration configuration, string key)
    {
        return configuration[key]
            ?? throw new InvalidOperationException($"Configuration value '{key}' is required.");
    }
}
