namespace music_time_manager.Core.Models;

public class SubtaskAssignee
{
    public SubtaskAssignee(Guid id, Guid subtaskId, Guid userId)
    {
        Id = id;
        SubtaskId = subtaskId;
        UserId = userId;
    }
    public Guid Id {get; private set;}
    public Guid SubtaskId { get; private set; }
    public Guid UserId { get; private set; }

    public static SubtaskAssignee Reconstitute(Guid id, Guid taskId, Guid userId)
    {
        return new SubtaskAssignee(id, taskId, userId);
    }
}