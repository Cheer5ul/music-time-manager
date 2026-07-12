namespace music_time_manager.Core.Models;

public class TaskAssignee
{
    public TaskAssignee(Guid id, Guid taskId, Guid userId)
    {
        Id = id;
        TaskId = taskId;
        UserId = userId;        
    }
    public Guid Id {get; private set;}
    public Guid TaskId { get; private set; }
    public Guid UserId { get; private set; }

    public static TaskAssignee Reconstitute(Guid id, Guid taskId, Guid userId)
    {
        return new TaskAssignee(id, taskId, userId);
    }
}