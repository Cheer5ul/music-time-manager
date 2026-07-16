namespace music_time_manager.Core.Errors;

public sealed class Error
{
    public Error(string code, string description)
    {
        Code = code;
        Description = description;
    }
    
    public string Code { get; }
    public string Description { get; }
    
    public static Error None => new Error(string.Empty, string.Empty); 
}