namespace music_time_manager.Persistence.Entities;

public class SubtaskAssigneeEntity
{
    public Guid Id {get; set;}
    public Guid SubtaskId { get; set; }
    public Guid UserId { get; set; }
}