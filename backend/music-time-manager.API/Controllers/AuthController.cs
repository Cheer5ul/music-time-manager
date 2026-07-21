using Microsoft.AspNetCore.Mvc;
using music_time_manager.Application.DTOs;
using music_time_manager.Application.Services;

namespace music_time_manager.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IFailureHandler _failureHandler;
    
    public AuthController(IUserService userService, 
        IFailureHandler failureHandler)
    {
        _userService = userService;
        _failureHandler = failureHandler;
    }
    
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterUserRequest registerUserRequest,
        CancellationToken ct)
    {
        var result = await _userService.Create(
            registerUserRequest.Name
            , registerUserRequest.Password,
            ct);
        
        if(result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);
        
        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginUserRequest loginUserRequest,
        CancellationToken ct)
    {
        var token = await _userService.Login(loginUserRequest.Username, loginUserRequest.Password, ct);
        
        if(token.IsFailure) return _failureHandler.HandleFailure(token, HttpContext);
        
        var response = token.Value;
        return Ok(response);
    }
}