using music_time_manager.Core.Errors;

namespace music_time_manager.Core.Result;

public class Result
{
    protected Result(bool isSuccess, List<Error> errors)
    {
        if (isSuccess && errors.Count != 0 ||
            !isSuccess && errors.Count == 0)
        {
            throw new ArgumentException("Invalid error", nameof(errors));
        }
        
        IsSuccess = isSuccess;
        Errors = errors;
    }
    private bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public List<Error> Errors { get; }

    public static Result Success => new Result(true, []);
    public static Result Failures(List<Error> errors) => new Result(false, errors); 
}