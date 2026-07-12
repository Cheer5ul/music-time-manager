namespace music_time_manager.Core.Models;

public class TaskAssignee
{
    public TaskAssignee(Guid taskId, Guid userId)
    {
        TaskId = taskId;
        UserId = userId;        
    }
    public Guid TaskId { get; private set; }
    public Guid UserId { get; private set; }

    public static TaskAssignee Reconstitute(Guid taskId, Guid userId)
    {
        return new TaskAssignee(taskId, userId);
    }
}