namespace music_time_manager.Persistence.Entities;

public class TaskAssigneeEntity
{
    public Guid Id {get; set;}
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
}