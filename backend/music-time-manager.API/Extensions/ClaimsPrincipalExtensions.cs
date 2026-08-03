using System.Security.Claims;

namespace music_time_manager.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirstValue("userId");
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}