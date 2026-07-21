using System.Runtime.InteropServices.JavaScript;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using music_time_manager.Core.Errors;
using music_time_manager.Core.Result;

namespace music_time_manager;

public class FailureHandler : IFailureHandler
{
    public ActionResult HandleFailure(Result result, HttpContext httpContext)
    {
        if (!result.IsFailure)
        {
            throw new InvalidOperationException("Cannot handle success result.");
        }

        var statusCode = GetStatusCode(result);
        var allErrors = GetAllErrors(result);

        var problem = new ProblemDetails()
        {
            Type = GetErrorType(result),
            Title = GetErrorTitle(result),
            Detail = GetErrorDetail(result),
            Status = statusCode,
            Instance = /*$"{httpContext.Request.Method}" + */
                       $"{httpContext.Request.Scheme}" +
                       $"://{httpContext.Request.Host}" +
                       $"{httpContext.Request.Path}",
            Extensions = GetErrorExtensions(allErrors)!
        };

        return new ObjectResult(problem);
    }

    private IReadOnlyList<Error> GetAllErrors(Result result)
    {
        var errors = new List<Error>();

        if (result.Errors.Count > 0)
        {
            errors.AddRange(result.Errors);
        }
        return errors;
    }

    private int GetStatusCode(Result result)
    {
        var errors = GetAllErrors(result);

        if (errors.Count == 0)
        {
            return StatusCodes.Status500InternalServerError;
        }

        foreach (var error in errors)
        {
            if (ErroCodeToStatus.TryGetValue(error.Code, out int statusCode))
            {
                return statusCode;
            }
        }
        
        return StatusCodes.Status400BadRequest;
    }

    private static readonly Dictionary<string, int> ErroCodeToStatus = new()
    {
        ["NotFound"] = StatusCodes.Status404NotFound,
        ["Forbidden"] = StatusCodes.Status403Forbidden,
        ["Unauthorized"] = StatusCodes.Status401Unauthorized,
        ["Conflict"] = StatusCodes.Status409Conflict,
        ["ValidationFailed"] = StatusCodes.Status400BadRequest,
    };

    // TODO: think of a better approach
    private string GetErrorType(Result result)
    {
        return result.Errors.Count < 1 ? result.Errors[0].Code : 
            "Unknown";
    }

    private string GetErrorTitle(Result result)
    {
        if(result.Errors.Count == 1)
        {
            return ToTitleCase(result.Errors[0].Code);
        }

        return "Multiple errors occured";
    }

    private static string ToTitleCase(string code)
    {
        var shortCode = code.Split('.').Last();
        return Regex.Replace(shortCode, "(?<!^)([A-Z])", " $1");
    }

    private string? GetErrorDetail(Result result)
    {
        if (result.Errors.Count == 1)
        {
            return result.Errors[0].Description;
        }

        return string.Join("; ", result.Errors.Select(e => e.Description));
    }

    private Dictionary<string, object>? GetErrorExtensions(IReadOnlyList<Error> errors)
    {
        if (errors.Count == 0)
        {
            return null;
        }

        var extensions = new Dictionary<string, object>();
        
        extensions["errors"] = errors.Select(error => 
            new Error(error.Code, error.Description)
        ).ToList();
        
        return extensions.Count > 0 ? extensions : null;
    }
}