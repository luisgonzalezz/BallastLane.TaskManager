using System.Reflection;

namespace BallastLane.TaskManager.Infrastructure.Tests;

public sealed class ProjectScaffoldTests
{
    [Fact]
    public void InfrastructureAssemblyLoads()
    {
        var assembly = Assembly.Load("BallastLane.TaskManager.Infrastructure");

        Assert.Equal("BallastLane.TaskManager.Infrastructure", assembly.GetName().Name);
    }
}
