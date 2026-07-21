using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using music_time_manager.API.DTOs;
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
    public async Task<ActionResult<UserResponse>> GetByUsername(string username, 
        CancellationToken ct)
    {
        var result = await _userService.GetByUsername(username, ct);
        
        if (result.Value == null) return _failureHandler.HandleFailure(result, HttpContext);
        
        var response = new UserResponse(result.Value.UserName);
        
        return Ok(response);
    } 

    [Authorize]
    [HttpDelete]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _userService.Delete(id, ct);
        
        return Ok();
    }
}