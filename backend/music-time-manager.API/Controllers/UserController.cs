using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using music_time_manager.API.DTOs;
using music_time_manager.API.Extensions;
using music_time_manager.Application.DTOs;
using music_time_manager.Application.Services;

namespace music_time_manager.API.Controllers;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IFailureHandler _failureHandler;
    
    public UserController(IUserService userService, 
        IFailureHandler failureHandler)
    {
        _userService = userService;
        _failureHandler = failureHandler;
    }

    [Authorize]
    [HttpGet]
    [EnableRateLimiting("per-user")]
    public async Task<ActionResult<List<UserResponseWithId>>> GetUsers(CancellationToken ct)
    {
        var result = await _userService.GetUsers(ct);
        
        if (result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);

        var response = result.Value!.Select(u =>
            new UserResponseWithId(u.Id, u.UserName)).ToList();
        
        return Ok(response);
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    [EnableRateLimiting("per-user")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _userService.GetById(id, ct);
        if (result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);
        
        var response = new UserResponse(result.Value!.UserName);
        
        return Ok(response);
    }


    [Authorize]
    [HttpGet("{id:guid}/stats")]
    [EnableRateLimiting("per-user")]
    public async Task<ActionResult<UserStatsResponse>> GetStats(Guid id, CancellationToken ct)
    {
        var result = await _userService.GetStats(id, ct);
        if (result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);

        var response = new UserStatsResponse(result.Value!.CompletedCount,
            result.Value!.MissedCound);

        return Ok(response);
    }

    [Authorize]
    [HttpPatch("{id:guid}/username")]
    [EnableRateLimiting("per-user")]
    public async Task<ActionResult> UpdateUsername([FromBody] UpdateUsernameRequest request,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        if(userId is null) return Unauthorized();
        
        var result = await _userService.UpdateUsername(userId.Value, request.NewUsername, ct);
        if(result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);
        
        return Ok();
    }

    [Authorize]
    [HttpPatch("{id:guid}/password")]
    [EnableRateLimiting("per-user")]
    public async Task<ActionResult> UpdatePassword(Guid id, [FromBody] UpdatePasswordRequest request,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        
        var result = await _userService.UpdatePassword(userId.Value, request.CurrentPassword,
            request.NewPassword, ct);
        if (result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);
        
        return Ok();
    }
}