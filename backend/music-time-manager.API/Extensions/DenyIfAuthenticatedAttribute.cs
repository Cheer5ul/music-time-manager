using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace music_time_manager.API.Extensions;

public class DenyIfAuthenticatedAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            context.Result = new ConflictObjectResult("You are already logged in.");
        }
    }
}