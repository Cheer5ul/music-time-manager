using Microsoft.AspNetCore.Mvc;
using music_time_manager.Application.DTOs;
using music_time_manager.Application.Services;
using music_time_manager.Core.Models;
using SneakerStore.FailureHandler;
using Task = music_time_manager.Core.Models.Task;

namespace music_time_manager.API.Controllers;

[ApiController]
[Route("tasks")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly IFailureHandler _failureHandler;
    public TaskController(ITaskService taskService,
        IFailureHandler failureHandler)
    {
        _taskService = taskService;
        _failureHandler = failureHandler;
    }
    
    [HttpPost]
    public async Task<ActionResult> CreateTask(
        [FromBody] TaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _taskService.CreateTask(
            request.Title,
            request.DueDate,
            request.CreatedBy,
            request.Description, 
            cancellationToken);
        
        if(result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);
        
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteTask(Guid id, CancellationToken ct = default)
    {
        var result = await _taskService.Delete(id, ct);
        
        if(result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);
            
        return Ok(result);
    }
}