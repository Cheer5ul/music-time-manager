using Microsoft.EntityFrameworkCore;
using music_time_manager.Core.Models;
using music_time_manager.Persistence.Entities;
using Task = System.Threading.Tasks.Task;

namespace music_time_manager.Persistence.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly MusicTimeManagerDbContext _dbContext;
    
    public TaskRepository(MusicTimeManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Core.Models.Task>> GetTasks(
        Status? status,
        bool? isOverdue,
        Guid? assigneeId,
        Guid? createdBy,
        DateTime? dueBefore,
        DateTime? dueAfter,
        bool? hasAssignees,
        CancellationToken ct = default)
    
    {
        var query = _dbContext.Tasks.AsNoTracking().AsQueryable();
        
        if(status.HasValue)
            query = query.Where(t => t.Status == status.Value);
        if (isOverdue.HasValue)
            query = isOverdue.Value
                ? query.Where(t => t.DueDate < DateTime.UtcNow && t.Status != Status.Done)
                : query.Where(t => t.DueDate >= DateTime.UtcNow || t.Status == Status.Done); 
        if(assigneeId.HasValue)
            query = query.Where(t => t.TaskAssignees.Any(ta => ta.UserId == assigneeId.Value));
        if(createdBy.HasValue)
            query = query.Where(t => t.CreatedBy == createdBy.Value);
        if (dueBefore.HasValue)
            query = query.Where(t => t.DueDate < dueBefore.Value);
        if (dueAfter.HasValue)
            query = query.Where(t => t.DueDate > dueAfter.Value);
        if(hasAssignees.HasValue)
            query = hasAssignees.Value
                ? query.Where(t => t.TaskAssignees.Any())
                : query.Where(t => !t.TaskAssignees.Any());
        
        
        var taskEntities = await query.ToListAsync(ct);

        var tasks = taskEntities
            .Select(te => Core.Models.Task.Reconstitute(
                te.Id,
                te.Title,
                te.DueDate,
                te.CreatedAt,
                te.Status,
                te.CreatedBy,
                new List<Subtask>(),
                te.Description,
                te.RecreatedFromTaskId))
            .ToList();
        
        return tasks;
    }

    public async Task<(Core.Models.Task task, IReadOnlyList<TaskAssignee> assignees)> GetTask(
        Guid taskId, CancellationToken ct = default)
    {
        var taskEntity = await _dbContext.Tasks
            .AsNoTracking()
            .FirstAsync(t => t.Id == taskId, ct);
        
        var subtaskEntities = await _dbContext.Subtasks
            .AsNoTracking()
            .Where(ste => ste.TaskId == taskId)
            .ToListAsync(ct);
        
        var taskAssigneeEntities = await _dbContext.TaskAssignees
            .AsNoTracking()
            .Where(tae => tae.TaskId == taskId)
            .Include(tae => tae.User)
            .ToListAsync(ct);
        
        var subtasks = subtaskEntities
            .Select(ste => Subtask.Reconstitute(
                ste.Id,
                ste.Title,
                ste.Status,
                ste.TaskId))
            .ToList();

        var taskAssignees = taskAssigneeEntities
            .Select(tae => TaskAssignee.Reconstitute(
                tae.TaskId,
                tae.UserId,
                tae.User.UserName))
            .ToList();

        var task = Core.Models.Task.Reconstitute(
            taskEntity.Id,
            taskEntity.Title,
            taskEntity.DueDate,
            taskEntity.CreatedAt,
            taskEntity.Status,
            taskEntity.CreatedBy,
            subtasks,
            taskEntity.Description,
            taskEntity.RecreatedFromTaskId);

        return (task, taskAssignees);
    }

    public async Task<(List<Subtask> subtasks, Dictionary<Guid, DateTime> dateTimes)> GetSubTasks(CancellationToken ct = default)
    {
        var subtaskEntities = await _dbContext.Subtasks
            .Include(st => st.Task)
            .AsNoTracking()
            .ToListAsync(ct);
        
        var subtasks = subtaskEntities
            .Select(se => Subtask.Reconstitute(
                se.Id,
                se.Title,
                se.Status,
                se.TaskId))
            .ToList();
        
        var dateTimes = new Dictionary<Guid, DateTime>();

        foreach (var stEntity in subtaskEntities)
        {
            dateTimes.Add(stEntity.Id, stEntity.Task.DueDate);
        }

        return (subtasks, dateTimes);
    }

    public async Task CreateTask(Core.Models.Task task, CancellationToken ct = default)
    {
        var taskEntity = new TaskEntity()
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            CreatedBy = task.CreatedBy,
            Status = task.Status,
            RecreatedFromTaskId = null,
            SubtaskEntities = new List<SubtaskEntity>()
        };
        
        await _dbContext.Tasks.AddAsync(taskEntity, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task ReplaceTaskAssignees(Guid taskId, List<TaskAssignee> assignees, CancellationToken ct = default)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();


        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            await _dbContext.TaskAssignees
                .Where(tae => tae.TaskId == taskId)
                .ExecuteDeleteAsync(ct);

            var entities = assignees.Select(a => new TaskAssigneeEntity()
            {
                TaskId = taskId,
                UserId = a.UserId
            });

            await _dbContext.TaskAssignees.AddRangeAsync(entities, ct);
            await _dbContext.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);
        });
    }

    public async Task ReplaceSubtaskAssignees(Guid subtaskId, List<SubtaskAssignee> assignees, CancellationToken ct = default)
    {
        await _dbContext.SubtaskAssignees
            .Where(ta => ta.SubtaskId == subtaskId)
            .ExecuteDeleteAsync(ct);

        var entities = assignees.Select(a => new SubtaskAssigneeEntity()
        {
            SubtaskId = subtaskId,
            UserId = a.UserId,
        });
        
        await _dbContext.SubtaskAssignees.AddRangeAsync(entities, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task CreateSubtask(Guid taskId, Subtask subtask, CancellationToken ct = default)
    {
        var subtaskEntity = new SubtaskEntity()
        {
            Title = subtask.Title,
            Id = subtask.Id,
            Status = subtask.Status,
            TaskId = taskId,
        };
        
        await _dbContext.Subtasks.AddAsync(subtaskEntity, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateStatus(Guid taskId, Status status, CancellationToken ct = default)
    {
        await _dbContext.Tasks.Where(t => t.Id == taskId)
            .ExecuteUpdateAsync(
                s => s.SetProperty(t => t.Status, status),
                ct);
    }

    public async Task UpdateTaskTitle(Guid taskId, string title, CancellationToken ct = default)
    {
        await _dbContext.Tasks.Where(t => t.Id == taskId)
            .ExecuteUpdateAsync(
                s => s.SetProperty(t => t.Title, title),
                ct);
    }

    public async Task UpdateTaskDescription(Guid taskId, string description, CancellationToken ct = default)
    {
        await _dbContext.Tasks.Where(t => t.Id == taskId)
            .ExecuteUpdateAsync(
                s => s.SetProperty(t => t.Description, description),
                ct);
    }

    public async Task UpdateTaskDueDate(Guid taskId, DateTime dueDate, CancellationToken ct = default)
    {
        await _dbContext.Tasks.Where(t => t.Id == taskId)
            .ExecuteUpdateAsync(
                s => s.SetProperty(t => t.DueDate, dueDate),
                ct);
    }

    public async Task<bool> DoesTaskExist(Guid taskId, CancellationToken ct = default)
    {
        var task =  await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == taskId, ct);

        return task != null;
    }

    public async Task<bool> DoesSubtaskExist(Guid subtaskId, CancellationToken ct = default)
    {
        var subtask = await _dbContext.Subtasks.FirstOrDefaultAsync(s => s.Id == subtaskId, ct);
        
        return subtask != null;
        
        // Or return await _dbContext.Subtasks.AnyAsync(s => s.Id == subtaskId, ct);
    }

    public async Task DeleteTask(Guid taskId, CancellationToken ct = default)
    {
        await _dbContext.Tasks.Where(te => te.Id == taskId)
            .ExecuteDeleteAsync(ct);
    }
}