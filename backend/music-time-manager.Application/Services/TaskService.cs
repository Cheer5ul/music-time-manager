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
    
    public async Task<Result> CreateTaskWithAssignees(Task task, List<Guid> taskAssignedUsersIds, 
        List<Guid> subtaskAssignedUsersIds,
        CancellationToken ct = default)
    {
        // task assignees
        List<TaskAssignee> taskAssignees = [];

        foreach (var assigneeId in taskAssignedUsersIds)
        {
            taskAssignees.Add(
                TaskAssignee.Reconstitute(task.Id, assigneeId));
        }
        
        // subtask assignees
        List<SubtaskAssignee> subtaskAssignees = [];

        foreach (var assigneeId in subtaskAssignedUsersIds)
        {
            foreach (var subtask in task.SubTasks)
            {
                subtaskAssignees.Add(
                    SubtaskAssignee.Reconstitute(subtask.TaskId, assigneeId));
            }
        }
        
        await _taskRepository.CreateTaskWithAssignees(task.Id, taskAssignees, subtaskAssignees, ct);
        
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