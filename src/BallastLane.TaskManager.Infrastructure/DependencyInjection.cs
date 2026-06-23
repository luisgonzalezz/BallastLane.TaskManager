using BallastLane.TaskManager.Application.Abstractions;
using BallastLane.TaskManager.Infrastructure.Data;
using BallastLane.TaskManager.Infrastructure.Repositories;
using BallastLane.TaskManager.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace BallastLane.TaskManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();

        return services;
    }
}
