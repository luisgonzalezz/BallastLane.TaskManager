using System.Security.Claims;
using BallastLane.TaskManager.Api.Contracts;
using BallastLane.TaskManager.Api.Contracts.Tasks;
using BallastLane.TaskManager.Application.Exceptions;
using BallastLane.TaskManager.Application.Services;
using BallastLane.TaskManager.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CreateTaskItemRequest = BallastLane.TaskManager.Application.DTOs.Tasks.CreateTaskItemRequest;
using UpdateTaskItemRequest = BallastLane.TaskManager.Application.DTOs.Tasks.UpdateTaskItemRequest;

namespace BallastLane.TaskManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class TasksController : ControllerBase
{
    private readonly ITaskItemService taskItemService;

    public TasksController(ITaskItemService taskItemService)
    {
        this.taskItemService = taskItemService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var tasks = await taskItemService.GetForUserAsync(userId, cancellationToken);

        return Ok(tasks);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTask(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var task = await taskItemService.GetByIdAsync(userId, id, cancellationToken);

            return Ok(task);
        }
        catch (ApplicationNotFoundException exception)
        {
            return NotFound(new ApiErrorResponse(exception.Message));
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(
        CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var task = await taskItemService.CreateAsync(
                userId,
                new CreateTaskItemRequest(request.Title, request.Description, request.DueDate),
                cancellationToken);

            return Ok(task);
        }
        catch (DomainValidationException exception)
        {
            return BadRequest(new ApiErrorResponse(exception.Message));
        }
        catch (ApplicationValidationException exception)
        {
            return BadRequest(new ApiErrorResponse(exception.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTask(
        Guid id,
        UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var task = await taskItemService.UpdateAsync(
                userId,
                id,
                new UpdateTaskItemRequest(request.Title, request.Description, request.DueDate, request.Status),
                cancellationToken);

            return Ok(task);
        }
        catch (ApplicationNotFoundException exception)
        {
            return NotFound(new ApiErrorResponse(exception.Message));
        }
        catch (DomainValidationException exception)
        {
            return BadRequest(new ApiErrorResponse(exception.Message));
        }
        catch (ApplicationValidationException exception)
        {
            return BadRequest(new ApiErrorResponse(exception.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTask(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            await taskItemService.DeleteAsync(userId, id, cancellationToken);

            return NoContent();
        }
        catch (ApplicationNotFoundException exception)
        {
            return NotFound(new ApiErrorResponse(exception.Message));
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            throw new UnauthorizedAccessException("Authenticated user id is missing or invalid.");
        }

        return userId;
    }
}
