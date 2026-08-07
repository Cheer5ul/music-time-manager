using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using music_time_manager.API.DTOs;
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
    public async Task<ActionResult<List<UserResponse>>> GetUsers(CancellationToken ct)
    {
        var result = await _userService.GetUsers(ct);
        
        if (result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);

        var response = result.Value!.Select(u =>
            new UserResponse(u.UserName)).ToList();
        
        return Ok(response);
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _userService.GetById(id, ct);
        if (result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);
        
        var response = new UserResponse(result.Value!.UserName);
        
        return Ok(response);
    }

    [Authorize]
    [HttpPatch("{id:guid}/username")]
    public async Task<ActionResult> UpdateUsername(Guid id, [FromBody] UpdateUsernameRequest request,
        CancellationToken ct)
    {
        var result = await _userService.UpdateUsername(id, request.NewUsername, ct);
        if(result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);
        
        return Ok();
    }
}