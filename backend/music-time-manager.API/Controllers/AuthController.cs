using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using music_time_manager.API.DTOs;
using music_time_manager.Application.DTOs;
using music_time_manager.Application.Services;
using music_time_manager.Infrastructure.Options;

namespace music_time_manager.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IFailureHandler _failureHandler;
    private readonly JwtOptions  _jwtOptions;
    
    public AuthController(IUserService userService, 
        IFailureHandler failureHandler,
        IOptions<JwtOptions> jwtOptions)
    {
        _userService = userService;
        _failureHandler = failureHandler;
        _jwtOptions = jwtOptions.Value;
    }
    
    [HttpPost("register")]
    [AllowAnonymous]
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
    [AllowAnonymous]
    public async Task<ActionResult> Login([FromBody] LoginUserRequest loginUserRequest,
        CancellationToken ct)
    {
        var result = await _userService.Login(loginUserRequest.Username, loginUserRequest.Password, ct);
        
        if(result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);
        
        var token = result.Value!;
        
        HttpContext.Response.Cookies.Append("access_token", token, new CookieOptions()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(_jwtOptions.ExpiresHours)
        }); 
        
        return Ok();
    }

    [HttpPost("logout")]
    [Authorize]
    public ActionResult Logout()
    {
        HttpContext.Response.Cookies.Delete("access_token");
        return Ok();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserResponseWithId>> Me(CancellationToken ct)
    {
        var userIdClaim = User.FindFirstValue("userId");
        if(userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var result = await _userService.GetById(userId, ct);
        if(result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);

        var user = result.Value!;

        return Ok(new UserResponseWithId(user.Id, user.UserName));
    }
}