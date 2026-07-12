using music_time_manager.Core.Models;

namespace music_time_manager.Persistence.Entities;

public class SubtaskEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Status Status { get; set; }
    public Guid TaskId { get; set; }
}