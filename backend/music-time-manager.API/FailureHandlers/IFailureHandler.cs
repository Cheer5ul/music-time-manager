using Microsoft.AspNetCore.Mvc;
using music_time_manager.Core.Result;

namespace music_time_manager;

public interface IFailureHandler
{
    ActionResult HandleFailure(Result result, HttpContext httpContext);
}