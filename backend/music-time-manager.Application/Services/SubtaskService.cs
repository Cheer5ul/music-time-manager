using music_time_manager.Core.Errors;
using music_time_manager.Core.Models;
using music_time_manager.Core.Result;
using music_time_manager.Persistence.Repositories;

namespace music_time_manager.Application.Services;

public class SubtaskService : ISubtaskService
{
    private readonly ITaskRepository _taskRepository;
    
    public SubtaskService(ITaskRepository repository)
    {
        _taskRepository = repository;
    }
    
    public async Task<ResultT<List<Subtask>>> GetSubTasks(CancellationToken ct = default)
    {
        var subtasks = await _taskRepository.GetSubTasks(ct);
        
        return ResultT<List<Subtask>>.Success(subtasks);
    }
    
    public async Task<Result> AssignUsersToSubtask(Guid subtaskId, List<Guid> userIds, CancellationToken ct = default)
    {
        if (userIds.Count == 0)
        {
            return Result.Failures([SubtaskErrors.MustHaveAtLeastOneAssignee()]);
        }
        
        var doesTaskExist = await _taskRepository.DoesTaskExist(subtaskId, ct);
        if (!doesTaskExist) return Result.Failures([SubtaskErrors.DoesNotExist(subtaskId)]);
        
        var assignees = userIds.
            Select(userId => SubtaskAssignee.Reconstitute(subtaskId, userId))
            .ToList();
            
        await _taskRepository.ReplaceSubtaskAssignees(subtaskId, assignees, ct);
        return Result.Success;
    }
}