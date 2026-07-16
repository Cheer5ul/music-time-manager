using Microsoft.AspNetCore.Mvc;
using music_time_manager.Core.Result;

namespace SneakerStore.FailureHandler;

public interface IFailureHandler
{
    ActionResult HandleFailure(Result result, HttpContext httpContext);
}