namespace music_time_manager.Core.Errors;

public static class SubTaskErrors 
{
    private static class Codes
    {
        public const string InvalidTitle = "Task.InvalidTtile";
    } 
    
    public static Error InvalidTitle(string title) =>
        new Error(Codes.InvalidTitle, 
            $"Title '{title}' is invalid.");
}