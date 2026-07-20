using Microsoft.AspNetCore.Mvc;
using music_time_manager.API.DTOs;
using music_time_manager.Application.Services;
using music_time_manager.Core.Models;
using SneakerStore.FailureHandler;

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

    [HttpGet]
    public async Task<ActionResult<List<Subtask>>> GetSubtasks(CancellationToken ct)
    {
        var subtasks = await _subtaskService.GetSubTasks(ct);
        
        if(subtasks.IsFailure) return _failureHandler.HandleFailure(subtasks, HttpContext);

        var response = subtasks.Value!
            .Select(s => new SubtaskResponse(
                Id: s.Id,
                Title: s.Title,
                Status: s.Status,
                TaskId: s.TaskId))
            .ToList();
        
        return Ok(response);
    }
}