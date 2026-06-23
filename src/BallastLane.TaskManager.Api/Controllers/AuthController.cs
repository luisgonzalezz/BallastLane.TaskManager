using BallastLane.TaskManager.Api.Contracts;
using BallastLane.TaskManager.Api.Contracts.Auth;
using BallastLane.TaskManager.Application.Exceptions;
using BallastLane.TaskManager.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegisterUserRequest = BallastLane.TaskManager.Application.DTOs.Auth.RegisterUserRequest;
using LoginServiceRequest = BallastLane.TaskManager.Application.DTOs.Auth.LoginRequest;

namespace BallastLane.TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService authService;

    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await authService.RegisterAsync(
                new RegisterUserRequest(request.Email, request.Password),
                cancellationToken);

            return Ok(response);
        }
        catch (ApplicationValidationException exception)
        {
            return BadRequest(new ApiErrorResponse(exception.Message));
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await authService.LoginAsync(
                new LoginServiceRequest(request.Email, request.Password),
                cancellationToken);

            return Ok(response);
        }
        catch (ApplicationValidationException exception)
        {
            return BadRequest(new ApiErrorResponse(exception.Message));
        }
    }

    [HttpGet("public")]
    public IActionResult Public()
    {
        return Ok(new { Message = "Public endpoint" });
    }

    [Authorize]
    [HttpGet("private")]
    public IActionResult Private()
    {
        return Ok(new { Message = "Private endpoint" });
    }
}
