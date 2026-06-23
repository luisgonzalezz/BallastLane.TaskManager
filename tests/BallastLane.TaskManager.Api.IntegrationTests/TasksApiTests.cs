using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BallastLane.TaskManager.Application.DTOs.Tasks;
using BallastLane.TaskManager.Application.Services;
using BallastLane.TaskManager.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace BallastLane.TaskManager.Api.IntegrationTests;

public sealed class TasksApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid UserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private readonly WebApplicationFactory<Program> factory;

    public TasksApiTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped<ITaskItemService, FakeTaskItemService>();
            });
        });
    }

    [Fact]
    public async Task GetTasks_WithoutToken_ReturnsUnauthorized()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/tasks");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetTasks_WithToken_ReturnsUserTasks()
    {
        var client = CreateAuthorizedClient();

        var response = await client.GetAsync("/api/tasks");
        var tasks = await response.Content.ReadFromJsonAsync<List<TaskItemResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var task = Assert.Single(tasks!);
        Assert.Equal(UserId, task.UserId);
        Assert.Equal("List task", task.Title);
    }

    [Fact]
    public async Task GetTaskById_WithToken_ReturnsTask()
    {
        var client = CreateAuthorizedClient();
        var taskId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        var response = await client.GetAsync($"/api/tasks/{taskId}");
        var task = await response.Content.ReadFromJsonAsync<TaskItemResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(taskId, task!.Id);
        Assert.Equal(UserId, task.UserId);
    }

    [Fact]
    public async Task CreateTask_WithToken_ReturnsCreatedTask()
    {
        var client = CreateAuthorizedClient();
        var dueDate = DateTime.UtcNow.AddDays(4);

        var response = await client.PostAsJsonAsync(
            "/api/tasks",
            new { Title = "Create task", Description = "Created by API", DueDate = dueDate });
        var task = await response.Content.ReadFromJsonAsync<TaskItemResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(UserId, task!.UserId);
        Assert.Equal("Create task", task.Title);
        Assert.Equal(TaskItemStatus.Pending, task.Status);
    }

    [Fact]
    public async Task UpdateTask_WithToken_ReturnsUpdatedTask()
    {
        var client = CreateAuthorizedClient();
        var taskId = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var dueDate = DateTime.UtcNow.AddDays(7);

        var response = await client.PutAsJsonAsync(
            $"/api/tasks/{taskId}",
            new { Title = "Updated task", Description = "Updated by API", DueDate = dueDate, Status = TaskItemStatus.Completed });
        var task = await response.Content.ReadFromJsonAsync<TaskItemResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(taskId, task!.Id);
        Assert.Equal(UserId, task.UserId);
        Assert.Equal("Updated task", task.Title);
        Assert.Equal(TaskItemStatus.Completed, task.Status);
    }

    [Fact]
    public async Task DeleteTask_WithToken_ReturnsNoContent()
    {
        var client = CreateAuthorizedClient();

        var response = await client.DeleteAsync("/api/tasks/66666666-6666-6666-6666-666666666666");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private HttpClient CreateAuthorizedClient()
    {
        var client = factory.CreateClient();
        var token = TestJwtTokenFactory.CreateToken(UserId);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }
}
