namespace music_time_manager.Core.Errors;

public sealed class Error
{
    public Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }
    
    public string Code { get; }
    public string Description { get; }
    public ErrorType Type { get; }
    
    // public static Error None => new Error(string.Empty, string.Empty); 
}