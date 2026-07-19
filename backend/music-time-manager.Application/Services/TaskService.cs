using System.Globalization;
using music_time_manager.Core.Errors;
using music_time_manager.Core.Models;
using music_time_manager.Core.Result;
using music_time_manager.Persistence.Repositories;
using Task = music_time_manager.Core.Models.Task;

namespace music_time_manager.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    
    public TaskService(ITaskRepository repository)
    {
        _taskRepository = repository;
    }

    public async Task<Result> CreateTask(string title, DateTime dueDate,
        Guid createdBy, string? description, CancellationToken ct = default)
    {
        var task = Task.Create(
            title,
            dueDate,
            createdBy,
            new List<Subtask>(),
            description,
            null);
        
        if(task.IsFailure) return Result.Failures(task.Errors);
        
        await  _taskRepository.CreateTask(task.Value!, ct);
        return Result.Success;
    }

    public async Task<Result> AssignUsersToTask(Guid taskId, List<Guid> userIds, CancellationToken ct = default)
    {
        if (userIds.Count == 0)
        {
            return Result.Failures([TaskErrors.MustHaveAtLeastOneAssignee()]);
        }
        
        var doesTaskExist = await _taskRepository.DoesTaskExist(taskId, ct);
        if(!doesTaskExist) return Result.Failures([TaskErrors.DoesNotExist(taskId)]);

        var assignees = userIds
            .Select(userId => TaskAssignee.Reconstitute(taskId, userId))
            .ToList();

        await _taskRepository.ReplaceTaskAssignees(taskId, assignees, ct);
        return Result.Success;
    }

    public async Task<Result> AssignUsersToSubtask(Guid subtaskId, List<Guid> userIds, CancellationToken ct = default)
    {
        if (userIds.Count == 0)
        {
            return Result.Failures([TaskErrors.MustHaveAtLeastOneAssignee()]);
        }
        
        var doesTaskExist = await _taskRepository.DoesTaskExist(subtaskId, ct);
        if (!doesTaskExist) return Result.Failures([TaskErrors.DoesNotExist(subtaskId)]);
        
        var assignees = userIds.
            Select(userId => SubtaskAssignee.Reconstitute(subtaskId, userId))
            .ToList();
            
        await _taskRepository.ReplaceSubtaskAssignees(subtaskId, assignees, ct);
        return Result.Success;
    }


    public async Task<Result> Delete(Guid taskId,
        CancellationToken ct = default)
    {
        var doesTaskExist = await _taskRepository.DoesTaskExist(taskId, ct);

        if (!doesTaskExist) return Result.Failures([TaskErrors.DoesNotExist(taskId)]);
        
        await _taskRepository.DeleteTask(taskId, ct);
        return Result.Success;
    }
}