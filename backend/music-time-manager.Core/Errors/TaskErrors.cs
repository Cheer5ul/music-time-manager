namespace music_time_manager.Core.Errors;

public static class TaskErrors
{
    private static class Codes
    {
        public const string InvalidTitle = "Task.InvalidTtile";
        public const string InvalidDescription = "Task.InvalidDescription";
        public const string InvalidDueDate = "Task.InvalidDueDate";
        public const string DoesNotExist = "Task.DoesNotExist";
        public const string MustHaveAtLeastOneAssignee = "Task.MustHaveAtLeastOneAssignee";
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

    public static Error DoesNotExist(Guid id) =>
        new Error(Codes.DoesNotExist,
            $"Task with id {id} does not exist.");
    public static Error MustHaveAtLeastOneAssignee() =>
        new Error(Codes.MustHaveAtLeastOneAssignee,
            $"Task must have at least one assignee.");
}