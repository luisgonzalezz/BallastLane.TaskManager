using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BallastLane.TaskManager.Application.DTOs.Auth;
using BallastLane.TaskManager.Application.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace BallastLane.TaskManager.Api.IntegrationTests;

public sealed class AuthApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;

    public AuthApiTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped<IAuthService, FakeAuthService>();
            });
        });
    }

    [Fact]
    public async Task PublicEndpoint_ReturnsOk()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/auth/public");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PrivateEndpoint_WithoutToken_ReturnsUnauthorized()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/auth/private");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PrivateEndpoint_WithToken_ReturnsOk()
    {
        var client = factory.CreateClient();
        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new { Email = "demo@ballastlane.com", Password = "Password123!" });
        var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.Token);
        var privateResponse = await client.GetAsync("/api/auth/private");

        Assert.Equal(HttpStatusCode.OK, privateResponse.StatusCode);
    }

    [Fact]
    public async Task Register_WithValidRequest_ReturnsAuthResponse()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/auth/register",
            new { Email = "new@ballastlane.com", Password = "Password123!" });
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("new@ballastlane.com", auth!.Email);
        Assert.False(string.IsNullOrWhiteSpace(auth.Token));
    }

    [Fact]
    public async Task Login_WithValidRequest_ReturnsAuthResponse()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new { Email = "demo@ballastlane.com", Password = "Password123!" });
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("demo@ballastlane.com", auth!.Email);
        Assert.False(string.IsNullOrWhiteSpace(auth.Token));
    }
}
