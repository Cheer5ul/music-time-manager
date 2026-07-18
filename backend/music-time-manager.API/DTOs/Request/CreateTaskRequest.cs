namespace music_time_manager.Application.DTOs;

public class CreateTaskRequest
{
    public TaskRequest Task { get; set; }
    public List<Guid> TaskAssignedUsers { get; set; }
    public List<Guid> SubtaskAssignedUser { get; set; }
}