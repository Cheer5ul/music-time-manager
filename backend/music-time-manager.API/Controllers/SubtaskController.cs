using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using music_time_manager.API.DTOs;
using music_time_manager.Application.DTOs;
using music_time_manager.Application.Services;
using CoreStatus = music_time_manager.Core.Models.Status;

namespace music_time_manager.API.Controllers;

[ApiController]
[Route("subtasks")]
public class SubtaskController : ControllerBase
{
    private readonly ISubtaskService _subtaskService;
    private readonly IFailureHandler _failureHandler;
    
    public SubtaskController(ISubtaskService subtaskService, 
        IFailureHandler failureHandler)
    {
        _subtaskService = subtaskService;
        _failureHandler = failureHandler;
    }

    [Authorize]
    [HttpGet]
    [EnableRateLimiting("per-user")]
    public async Task<ActionResult<List<SubtaskResponse>>> GetSubtasks([FromQuery] SubtaskRequestFilter request,
        CancellationToken ct)
    {
        var subtasks = await _subtaskService.GetSubtasks(
            request.Status,
            request.IsOverdue,
            request.AssigneeId,
            request.TaskId,
            ct);
        
        if(subtasks.IsFailure) return _failureHandler.HandleFailure(subtasks, HttpContext);

        var response = subtasks.Value.subtasks!
            .Select(s => new SubtaskResponse(
                Id: s.Id,
                Title: s.Title,
                Status: s.Status,
                IsOverdue: subtasks.Value.dateTimes[s.Id] < DateTime.Now && s.Status != CoreStatus.Done,
                TaskId: s.TaskId))
            .ToList();
        
        return Ok(response);
    }
    
    [Authorize]
    [HttpPost("/tasks/{taskId:guid}/subtasks")]
    [EnableRateLimiting("per-user")]
    public async Task<ActionResult> CreateSubtask(Guid taskId, [FromBody] CreateSubtaskRequest subtaskRequest,
        CancellationToken ct)
    {
        var result = await _subtaskService.CreateSubtask(taskId, subtaskRequest.Title, ct);
        if(result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);
        
        return Ok();
    }

    [Authorize]
    [HttpPatch("{id:guid}")]
    [EnableRateLimiting("per-user")]
    public async Task<ActionResult> UpdateSubtaskTitle(Guid id, [FromBody] UpdateSubtaskTitleRequest request,
        CancellationToken ct)
    {
        var result = await _subtaskService.UpdateSubtaskTitle(id, request.NewTitle, ct);
        if(result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);
        
        return Ok();
    }

    [Authorize]
    [HttpPatch("{id:guid}/status")]
    [EnableRateLimiting("per-user")]
    public async Task<ActionResult> UpdateSubtaskStatus(Guid id, [FromQuery] UpdateStatusRequest request,
        CancellationToken ct)
    {
        var result = await _subtaskService.UpdateSubtaskStatus(id, request.Status, ct);
        if(result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);
        
        return Ok();
    }
    
    [Authorize]
    [HttpPut("{id:guid}/assignees")]
    [EnableRateLimiting("per-user")]
    public async Task<ActionResult> AssignUsers(Guid id, [FromBody] AssigneesUpdateRequest request,
        CancellationToken ct)
    {
        var result = await _subtaskService.AssignUsersToSubtask(id, request.UserIds, ct);
        if(result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);
        
        return Ok();
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    [EnableRateLimiting("per-user")]
    public async Task<ActionResult> DeleteSubtask(Guid id, CancellationToken ct)
    {
        var result = await _subtaskService.DeleteSubtask(id, ct);
        if (result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);
        
        return Ok();
    }
}