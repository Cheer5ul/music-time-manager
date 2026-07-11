namespace music_time_manager.Core.Errors;

public static class TaskErrors
{
    private static class Codes
    {
        public const string InvalidTitle = "Task.InvalidTtile";
        public const string InvalidDescription = "Task.InvalidDescription";
        public const string InvalidDueDate = "Task.InvalidDueDate";
    }    
    public static Error InvalidTitle(string title) =>
        new Error(Codes.InvalidTitle, 
            $"Title '{title}' is invalid.");
    public static Error InvalidDescription(string description) => 
        new Error(Codes.InvalidDescription,
                $"Description '{description}' is invalid.");
    public static Error InvalidDueDate(DateTime dueDate) =>
        new Error(Codes.InvalidDueDate,
            $"DueDate '{dueDate}' is invalid.");
}