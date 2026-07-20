namespace music_time_manager.Core.Errors;

public static class SubtaskErrors 
{
    private static class Codes
    {
        public const string InvalidTitle = "Subtask.InvalidTtile";
        public const string MustHaveAtLeastOneAssignee = "Subtask.MustHaveAtLeastOneAssignee";
        public const string DoesNotExist = "Subtask.DoesNotExist";
    } 
    
    public static Error InvalidTitle(string title) =>
        new Error(Codes.InvalidTitle, 
            $"Title '{title}' is invalid.");
    public static Error MustHaveAtLeastOneAssignee() =>
        new Error(Codes.MustHaveAtLeastOneAssignee,
            $"Task must have at least one assignee.");

    public static Error DoesNotExist(Guid id) =>
        new Error(Codes.DoesNotExist,
            $"Subtask with id {id} does not exist.");
}