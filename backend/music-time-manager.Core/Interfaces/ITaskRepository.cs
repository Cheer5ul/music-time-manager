using music_time_manager.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace music_time_manager.Persistence.Repositories;

public interface ITaskRepository
{
    Task<List<Core.Models.Task>> GetTasks(
        Status? status,
        bool? isOverdue,
        Guid? assigneeId,
        Guid? createdBy,
        DateTime? dueBefore,
        DateTime? dueAfter,
        bool? hasAssignees,
        CancellationToken ct = default);
    /// <remarks>
    /// The method does not check the validity and existence of the Task with given id.
    /// </remarks>
    Task<(Core.Models.Task task, IReadOnlyList<TaskAssignee> assignees)> GetTask(
        Guid taskId, CancellationToken ct = default);
    Task<(List<Subtask> subtasks, Dictionary<Guid, DateTime> dateTimes)> GetSubTasks(CancellationToken ct = default);
    Task CreateTask(Core.Models.Task task, CancellationToken ct = default);
    Task ReplaceTaskAssignees(Guid taskId, List<TaskAssignee> assignees, CancellationToken ct = default);
    Task ReplaceSubtaskAssignees(Guid subtaskId, List<SubtaskAssignee> assignees, CancellationToken ct = default);
    Task CreateSubtask(Guid taskId, Subtask subtask, CancellationToken ct = default);
    Task UpdateStatus(Guid taskId, Status status, CancellationToken ct = default);
    Task UpdateTaskTitle(Guid taskId, string title, CancellationToken ct = default);
    Task UpdateTaskDescription(Guid taskId, string description, CancellationToken ct = default);
    Task UpdateTaskDueDate(Guid taskId, DateTime dueDate, CancellationToken ct = default);
    Task<bool> DoesTaskExist(Guid taskId, CancellationToken ct = default);
    Task<bool> DoesSubtaskExist(Guid subtaskId, CancellationToken ct = default);
    Task DeleteTask(Guid taskId, CancellationToken ct = default);
}