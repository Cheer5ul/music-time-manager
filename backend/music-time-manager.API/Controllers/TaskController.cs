using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using music_time_manager.API.DTOs;
using music_time_manager.Application.DTOs;
using music_time_manager.Application.Services;
using CoreStatus = music_time_manager.Core.Models.Status;

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

    // [Authorize]
    // [HttpGet]
    // public async Task<ActionResult<List<TaskResponse>>> GetTasks(CancellationToken ct)
    // {
    //     var result = await _taskService.GetTasks(ct);
    //     
    //     if(result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);
    //
    //     var response = result.Value!
    //         .Select(t => new TaskResponse(
    //             Id: t.Id,
    //             Title: t.Title,
    //             Description: t.Description,
    //             DueDate: t.DueDate,
    //             CreatedBy: t.CreatedBy,
    //             CreatedAt: t.CreatedAt,
    //             Status: t.Status,
    //             IsOverdue: t.DueDate < DateTime.UtcNow && t.Status != CoreStatus.Done,
    //             RecreatedFromTaskId: t.RecreatedFromTaskId))
    //         .ToList();
    //     
    //     return Ok(response);
    // }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> GetTask(Guid id, CancellationToken ct)
    {
        var result = await _taskService.GetTask(id, ct);
        if(result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);

        var assigneesResponse = result.Value.assignees
            .Select(ta => new UserResponseWithId(
                ta.UserId,
                ta.UserName))
            .ToList();
        
        var subtasksResponse = result.Value.task.SubTasks
            .Select(st => new SubtaskResponse(
                st.Id,
                st.Title,
                result.Value.task.Status,
                result.Value.task.DueDate < DateTime.UtcNow && result.Value.task.Status != CoreStatus.Done,
                result.Value.task.Id))
            .ToList();
        
        var response = new TaskResponse(
            result.Value.task.Id,
            result.Value.task.Title,
            result.Value.task.Description,
            result.Value.task.DueDate,
            result.Value.task.CreatedAt,
            result.Value.task.Status,
            result.Value.task.DueDate < DateTime.UtcNow && result.Value.task.Status != CoreStatus.Done,
            result.Value.task.CreatedBy,
            result.Value.task.RecreatedFromTaskId,
            assigneesResponse,
            subtasksResponse);
        
        return Ok(response);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> CreateTask(
        [FromBody] TaskRequest request,
        CancellationToken ct)
    {
        var result = await _taskService.CreateTask(
            request.Title,
            request.DueDate,
            request.CreatedBy,
            request.Description, 
            ct);
        
        if(result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);
        
        return Ok();
    }

    [Authorize]
    [HttpPut("{id:guid}/assignees")]
    public async Task<ActionResult> AssignUsers(Guid id, [FromBody] AssigneesUpdateRequest request,
        CancellationToken ct)
    {
        var result = await _taskService.AssignUsersToTask(id, request.UserIds, ct);
        if(result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);
        
        return Ok();
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteTask(Guid id, CancellationToken ct)
    {
        var result = await _taskService.Delete(id, ct);
        
        if(result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);
            
        return Ok(result);
    }
}