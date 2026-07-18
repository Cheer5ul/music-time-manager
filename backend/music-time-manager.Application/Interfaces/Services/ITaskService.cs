using music_time_manager.Core.Models;
using music_time_manager.Core.Result;
using Task = music_time_manager.Core.Models.Task;

namespace music_time_manager.Application.Services;

public interface ITaskService
{
    Task<Result> CreateTask(string title, DateTime dueDate,
        Guid createdBy, string? description, CancellationToken ct = default);
    Task<Result> CreateTaskWithAssignees(Task task, List<Guid> taskAssignedUsersIds,
        List<Guid> subtaskAssignedUsersIds,
        CancellationToken ct = default);

    Task<Result> Delete(Guid taskId,
        CancellationToken ct = default);
}