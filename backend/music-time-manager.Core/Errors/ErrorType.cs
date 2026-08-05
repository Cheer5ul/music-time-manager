namespace music_time_manager.Core.Errors;

public enum ErrorType
{
    Validation, 
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    DomainInvariantViolation
}