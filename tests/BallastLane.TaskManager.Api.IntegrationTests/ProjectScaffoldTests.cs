using System.Reflection;

namespace BallastLane.TaskManager.Api.IntegrationTests;

public sealed class ProjectScaffoldTests
{
    [Fact]
    public void ApiAssemblyLoads()
    {
        var assembly = Assembly.Load("BallastLane.TaskManager.Api");

        Assert.Equal("BallastLane.TaskManager.Api", assembly.GetName().Name);
    }
}
