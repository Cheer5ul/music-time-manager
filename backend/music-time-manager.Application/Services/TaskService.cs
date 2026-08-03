using System.Globalization;
using music_time_manager.Core.Errors;
using music_time_manager.Core.Errors.User;
using music_time_manager.Core.Models;
using music_time_manager.Core.Result;
using music_time_manager.Persistence.Repositories;
using Task = music_time_manager.Core.Models.Task;

namespace music_time_manager.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    
    public TaskService(ITaskRepository repository,
        IUserRepository userRepository)
    {
        _taskRepository = repository;
        _userRepository = userRepository;
    }

    public async Task<ResultT<List<Task>>> GetTasks(
        Status? status,
        bool? isOverdue,
        Guid? assigneeId,
        Guid? createdBy,
        DateTime? dueBefore,
        DateTime? dueAfter,
        bool? hasAssignees,
        CancellationToken ct = default)
    {
        var tasks = await _taskRepository.GetTasks(
            status,
            isOverdue,
            assigneeId,
            createdBy,
            dueBefore,
            dueAfter,
            hasAssignees, 
            ct);

        return ResultT<List<Task>>.Success(tasks);
    }

    public async Task<ResultT<(Task task, IReadOnlyList<TaskAssignee> assignees)>> GetTask(
        Guid taskId, CancellationToken ct = default)
    {
        var doesTaskExist = await _taskRepository.DoesTaskExist(taskId, ct);
        if(doesTaskExist == false) return ResultT<(Task task, IReadOnlyList<TaskAssignee> assignees)>
            .Failures([TaskErrors.DoesNotExist(taskId)]);
        
        var taskAndAssignees = await _taskRepository.GetTask(taskId, ct);
        
        return ResultT<(Task task, IReadOnlyList<TaskAssignee> assignees)>.Success(taskAndAssignees);
    }

    public async Task<Result> CreateTask(string title, DateTime dueDate,
        Guid createdBy, string? description, CancellationToken ct = default)
    {
        var doesUserExist = await _userRepository.GetById(createdBy, ct);
        if(doesUserExist is null) return Result.Failures([UserErrors.DoesNotExits(createdBy)]);
        
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
            .Select(userId => TaskAssignee.Reconstitute(taskId, userId, null))
            .ToList();

        await _taskRepository.ReplaceTaskAssignees(taskId, assignees, ct);
        return Result.Success;
    }

    public async Task<Result> UpdateStatus(Guid taskId, Status status, CancellationToken ct = default)
    {
        var doesTaskExist = await _taskRepository.DoesTaskExist(taskId, ct);
        if(!doesTaskExist) return Result.Failures([TaskErrors.DoesNotExist(taskId)]);
        
        await _taskRepository.UpdateStatus(taskId, status, ct);
        return Result.Success;
    }

    public async Task<Result> UpdateTaskTitle(
        Guid id, string? title,
        CancellationToken ct = default)
    {
        var doesTaskExist = await _taskRepository.DoesTaskExist(id, ct);
        if(!doesTaskExist) return Result.Failures([TaskErrors.DoesNotExist(id)]);
        
        var result = Task.UpdateTitle(title);
        if(result.IsFailure) return Result.Failures(result.Errors);

        await _taskRepository.UpdateTaskTitle(id, title, ct);
        return Result.Success;
    }

    public async Task<Result> UpdateTaskDescription(
        Guid id, string? description, CancellationToken ct = default)
    {
        var doesTaskExist = await _taskRepository.DoesTaskExist(id, ct);
        if(!doesTaskExist) return Result.Failures([TaskErrors.DoesNotExist(id)]);
        
        var result = Task.UpdateDescription(description);
        if(result.IsFailure) return Result.Failures(result.Errors);
        
        await _taskRepository.UpdateTaskDescription(id, description, ct);
        return Result.Success;
    }

    public async Task<Result> UpdateTaskDueDate(
        Guid id, DateTime? dueDate, CancellationToken ct = default)
    {
        var doesTaskExist = await _taskRepository.DoesTaskExist(id, ct);
        if(!doesTaskExist) return Result.Failures([TaskErrors.DoesNotExist(id)]);
        
        var result = Task.UpdateDueDate(dueDate);
        if(result.IsFailure) return Result.Failures(result.Errors);
        
        await _taskRepository.UpdateTaskDueDate(id, dueDate.Value, ct);
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