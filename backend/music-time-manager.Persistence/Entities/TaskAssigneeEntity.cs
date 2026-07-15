namespace music_time_manager.Persistence.Entities;

public class TaskAssigneeEntity
{
    public Guid TaskId { get; set; }
    public TaskEntity Task { get; set; }
    public Guid UserId { get; set; }
    public UserEntity User { get; set; }
}