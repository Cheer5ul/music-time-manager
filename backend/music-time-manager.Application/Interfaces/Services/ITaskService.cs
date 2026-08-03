using music_time_manager.Core.Models;
using music_time_manager.Core.Result;
using Task = music_time_manager.Core.Models.Task;

namespace music_time_manager.Application.Services;

public interface ITaskService
{
    Task<ResultT<List<Task>>> GetTasks(
        Status? status,
        bool? isOverdue,
        Guid? assigneeId,
        Guid? createdBy,
        DateTime? dueBefore,
        DateTime? dueAfter,
        bool? hasAssignees,
        CancellationToken ct = default);
    Task<ResultT<(Task task, IReadOnlyList<TaskAssignee> assignees)>> GetTask(
        Guid taskId, CancellationToken ct = default);
    Task<Result> CreateTask(string title, DateTime dueDate,
        Guid createdBy, string? description, CancellationToken ct = default);
    Task<Result> AssignUsersToTask(Guid taskId, List<Guid> userIds, CancellationToken ct = default);
    Task<Result> UpdateStatus(Guid taskId, Status status, CancellationToken ct = default);
    Task<Result> Delete(Guid taskId, CancellationToken ct = default);
}