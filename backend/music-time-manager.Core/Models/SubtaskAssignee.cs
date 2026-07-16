namespace music_time_manager.Core.Models;

public class SubtaskAssignee
{
    public SubtaskAssignee(Guid subtaskId, Guid userId)
    {
        SubtaskId = subtaskId;
        UserId = userId;
    }
    public Guid SubtaskId { get; private set; }
    public Guid UserId { get; private set; }

    public static SubtaskAssignee Reconstitute(Guid taskId, Guid userId)
    {
        return new SubtaskAssignee(taskId, userId);
    }
}