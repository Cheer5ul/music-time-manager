using music_time_manager.Core.Models;
using music_time_manager.Core.Result;

namespace music_time_manager.Application.Services;

public interface ISubtaskService
{
    Task<ResultT<(List<Subtask> subtasks, Dictionary<Guid, DateTime> dateTimes)>> GetSubtasks(
        Status? status,
        bool? isOverdue,
        Guid? assigneeId,
        Guid? taskId,
        CancellationToken ct = default);
    Task<Result> CreateSubtask(Guid taskId, string subtaskTitle, CancellationToken ct = default);
    Task<Result> UpdateSubtaskTitle(Guid subtaskId, string newTile, CancellationToken ct = default);
    Task<Result> UpdateSubtaskStatus(Guid subtaskId, Status status, CancellationToken ct = default);
    Task<Result> AssignUsersToSubtask(Guid subtaskId, List<Guid> userIds, CancellationToken ct = default);
    Task<Result> DeleteSubtask(Guid subtaskId, CancellationToken ct = default);
}