using music_time_manager.Core.Models;

namespace music_time_manager.Persistence.Entities;

public class TaskEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public Status Status { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? RecreatedFromTaskId { get; set; }
    public TaskEntity? RecreatedFromTask { get; set; }
    public ICollection<TaskEntity> RecreatedTasks { get; set; } 
    public ICollection<SubtaskEntity> SubTasks { get; set; } 
}