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
        await _dbContext.TaskAssignees
            .Where(ta => ta.TaskId == taskId)
            .ExecuteDeleteAsync(ct);

        var entities = assignees.Select(a => new TaskAssigneeEntity()
        {
            TaskId = taskId,
            UserId = a.UserId,
        });
        
        await _dbContext.TaskAssignees.AddRangeAsync(entities, ct);
        await _dbContext.SaveChangesAsync(ct);
    }


    public async Task<bool> DoesTaskExist(Guid taskId, CancellationToken ct = default)
    {
        var task =  await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == taskId, ct);

        return task != null;
    }

    public async Task DeleteTask(Guid taskId, CancellationToken ct = default)
    {
        await _dbContext.Tasks.Where(te => te.Id == taskId)
            .ExecuteDeleteAsync(ct);
    }
}