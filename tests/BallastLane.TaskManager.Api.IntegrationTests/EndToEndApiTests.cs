using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BallastLane.TaskManager.Application.DTOs.Auth;
using BallastLane.TaskManager.Application.DTOs.Tasks;
using BallastLane.TaskManager.Domain.Enums;

namespace BallastLane.TaskManager.Api.IntegrationTests;

public sealed class EndToEndApiTests : IClassFixture<EndToEndApiFixture>
{
    private readonly EndToEndApiFixture fixture;

    public EndToEndApiTests(EndToEndApiFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task AuthAndTaskCrud_UsesRealServicesAndLocalDb()
    {
        var client = fixture.Factory.CreateClient();
        var email = $"e2e-{Guid.NewGuid():N}@ballastlane.com";
        const string password = "Password123!";

        var registerResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new { Email = email, Password = password });
        var registered = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();

        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);
        Assert.Equal(email, registered!.Email);
        Assert.False(string.IsNullOrWhiteSpace(registered.Token));

        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new { Email = email, Password = password });
        var loggedIn = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.Equal(registered.UserId, loggedIn!.UserId);
        Assert.False(string.IsNullOrWhiteSpace(loggedIn.Token));

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loggedIn.Token);
        var dueDate = DateTime.UtcNow.AddDays(3);
        var createResponse = await client.PostAsJsonAsync(
            "/api/tasks",
            new { Title = "E2E task", Description = "Created through real API", DueDate = dueDate });
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        Assert.Equal(registered.UserId, createdTask!.UserId);
        Assert.Equal("E2E task", createdTask.Title);
        Assert.Equal(TaskItemStatus.Pending, createdTask.Status);

        var listResponse = await client.GetAsync("/api/tasks");
        var listedTasks = await listResponse.Content.ReadFromJsonAsync<List<TaskItemResponse>>();

        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var listedTask = Assert.Single(listedTasks!);
        Assert.Equal(createdTask.Id, listedTask.Id);

        var getResponse = await client.GetAsync($"/api/tasks/{createdTask.Id}");
        var fetchedTask = await getResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal(createdTask.Id, fetchedTask!.Id);

        var updatedDueDate = DateTime.UtcNow.AddDays(5);
        var updateResponse = await client.PutAsJsonAsync(
            $"/api/tasks/{createdTask.Id}",
            new
            {
                Title = "Updated E2E task",
                Description = "Updated through real API",
                DueDate = updatedDueDate,
                Status = TaskItemStatus.Completed
            });
        var updatedTask = await updateResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.Equal("Updated E2E task", updatedTask!.Title);
        Assert.Equal(TaskItemStatus.Completed, updatedTask.Status);

        var deleteResponse = await client.DeleteAsync($"/api/tasks/{createdTask.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getDeletedResponse = await client.GetAsync($"/api/tasks/{createdTask.Id}");

        Assert.Equal(HttpStatusCode.NotFound, getDeletedResponse.StatusCode);
    }
}
