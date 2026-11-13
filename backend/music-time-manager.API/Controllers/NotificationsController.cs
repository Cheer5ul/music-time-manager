using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using music_time_manager.API.Extensions;
using music_time_manager.Application.Interfaces.Services;
using music_time_manager.Application.Services;

namespace music_time_manager.API.Controllers;

[ApiController]
[Route("notifications")]
public class NotificationsController : ControllerBase
{
    private readonly IFailureHandler _failureHandler;
    private readonly ITelegramLinkService _tgLinkService;

    public NotificationsController(
        ITelegramLinkService tgLinkService,
        IFailureHandler failureHandler)
    {
        _tgLinkService = tgLinkService;
        _failureHandler = failureHandler;
    }

    [Authorize]
    [HttpPost("telegram/link")]
    [EnableRateLimiting("per-user")]
    public async Task<ActionResult> CreateTgLinkToken(CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        
        var result = await _tgLinkService.CreateLinkToken(userId.Value, ct);
        
        if(result.IsFailure) return _failureHandler.HandleFailure(result,  HttpContext);
        
        return Ok(result.Value);
    }

    [Authorize]
    [HttpPost("telegram/unlink")]
    [EnableRateLimiting("per-user")]
    public async Task<ActionResult> UnlinkAccount(CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _tgLinkService.UnlinkTgAccount(userId.Value, ct);
        
        if(result.IsFailure) return _failureHandler.HandleFailure(result,  HttpContext);
        
        return Ok(result.Value);
    }
}