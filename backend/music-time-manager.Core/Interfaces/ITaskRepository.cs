using music_time_manager.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace music_time_manager.Persistence.Repositories;

public interface ITaskRepository
{
    /// <summary>
    /// Saves TaskEntity into DB, also saving Subtasks, TaskAssignees and SubtaskAssignees
    /// </summary>
    /// <param name="task"></param>
    /// <param name="taskAssignees"></param>
    /// <param name="subtaskAssignees"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task CreateTask(Core.Models.Task task, List<TaskAssignee> taskAssignees, 
        List<SubtaskAssignee> subtaskAssignees, CancellationToken ct = default);

    Task DeleteTask(Guid taskId, CancellationToken ct = default);
}