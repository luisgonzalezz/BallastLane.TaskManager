using BallastLane.TaskManager.Application.DTOs.Auth;
using BallastLane.TaskManager.Application.Services;
using BallastLane.TaskManager.Domain.Entities;
using BallastLane.TaskManager.Infrastructure.Security;
using Microsoft.Extensions.Configuration;

namespace BallastLane.TaskManager.Api.IntegrationTests;

public sealed class FakeAuthService : IAuthService
{
    private readonly JwtTokenGenerator tokenGenerator;

    public FakeAuthService(IConfiguration configuration)
    {
        tokenGenerator = new JwtTokenGenerator(configuration);
    }

    public Task<AuthResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(CreateResponse(request.Email));
    }

    public Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(CreateResponse(request.Email));
    }

    private AuthResponse CreateResponse(string email)
    {
        var user = User.FromPersistence(Guid.Parse("11111111-1111-1111-1111-111111111111"), email, "hash", DateTime.UtcNow);
        var token = tokenGenerator.GenerateToken(user);

        return new AuthResponse(user.Id, user.Email, token);
    }
}
