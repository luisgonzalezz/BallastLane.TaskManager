using BallastLane.TaskManager.Application.Abstractions;
using BallastLane.TaskManager.Application.DTOs.Auth;
using BallastLane.TaskManager.Application.Exceptions;
using BallastLane.TaskManager.Domain.Entities;

namespace BallastLane.TaskManager.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository userRepository;
    private readonly IPasswordHasher passwordHasher;
    private readonly IJwtTokenGenerator jwtTokenGenerator;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        this.userRepository = userRepository;
        this.passwordHasher = passwordHasher;
        this.jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponse> RegisterAsync(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ApplicationValidationException("Password is required.");
        }

        var existingUser = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
        {
            throw new ApplicationValidationException("Email is already registered.");
        }

        var passwordHash = passwordHasher.Hash(request.Password);
        var user = User.Create(request.Email, passwordHash);

        await userRepository.AddAsync(user, cancellationToken);

        return CreateResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new ApplicationValidationException("Invalid email or password.");
        }

        return CreateResponse(user);
    }

    private AuthResponse CreateResponse(User user)
    {
        var token = jwtTokenGenerator.GenerateToken(user);

        return new AuthResponse(user.Id, user.Email, token);
    }
}
