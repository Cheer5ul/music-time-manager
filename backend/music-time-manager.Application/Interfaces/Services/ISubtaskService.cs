using music_time_manager.Core.Models;
using music_time_manager.Core.Result;

namespace music_time_manager.Application.Services;

public interface ISubtaskService
{
    Task<ResultT<List<Subtask>>> GetSubTasks(CancellationToken ct = default);
    Task<Result> AssignUsersToSubtask(Guid subtaskId, List<Guid> userIds, CancellationToken ct = default);
}