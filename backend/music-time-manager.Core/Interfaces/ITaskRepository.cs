using music_time_manager.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace music_time_manager.Persistence.Repositories;

public interface ITaskRepository
{
    Task CreateTask(Core.Models.Task task, CancellationToken ct = default);

    Task ReplaceTaskAssignees(Guid taskId, List<TaskAssignee> assignees, CancellationToken ct = default);
    Task ReplaceSubtaskAssignees(Guid subtaskId, List<SubtaskAssignee> assignees, CancellationToken ct = default);

    Task<bool> DoesTaskExist(Guid taskId, CancellationToken ct = default);
    
    Task DeleteTask(Guid taskId, CancellationToken ct = default);
}