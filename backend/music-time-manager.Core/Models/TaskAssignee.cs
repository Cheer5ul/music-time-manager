namespace music_time_manager.Core.Models;

public class TaskAssignee
{
    public TaskAssignee(Guid taskId, Guid userId, string? userName)
    {
        TaskId = taskId;
        UserId = userId;       
        UserName = userName;
    }
    public Guid TaskId { get; private set; }
    public Guid UserId { get; private set; }
    public string? UserName { get; private set; }

    public static TaskAssignee Reconstitute(Guid taskId, Guid userId, string? userName)
    {
        return new TaskAssignee(taskId, userId, userName); 
    }
}