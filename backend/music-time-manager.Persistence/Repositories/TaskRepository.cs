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

    public async Task CreateTask(Core.Models.Task task, List<TaskAssignee> taskAssignees, 
        List<SubtaskAssignee> subtaskAssignees, CancellationToken ct = default)
    {
        var taskAssigneeEntities = taskAssignees.Select(
            ta => new TaskAssigneeEntity()
            {
                TaskId = ta.TaskId,
                UserId = ta.UserId
            }).ToList();

        var subtaskAssigneeEntities = subtaskAssignees.Select(
            sta => new SubtaskAssigneeEntity()
            {
                SubtaskId = sta.SubtaskId,
                UserId = sta.UserId
            }).ToList();
        
        var subtaskEntities = task.SubTasks
            .Select(st => new SubtaskEntity()
            {
                Id = st.Id,
                Title = st.Title,
                Status = st.Status,
                TaskId = st.TaskId
            })
            .ToList();
        
        var taskEntity = new TaskEntity()
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            Status = task.Status,
            CreatedBy = task.CreatedBy,
            SubtaskEntities = subtaskEntities
        };

        await _dbContext.TaskAssignees.AddRangeAsync(taskAssigneeEntities, ct);
        await _dbContext.SubtaskAssignees.AddRangeAsync(subtaskAssigneeEntities, ct);
        await _dbContext.Subtasks.AddRangeAsync(subtaskEntities, ct);
        await _dbContext.Tasks.AddAsync(taskEntity, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteTask(Guid taskId, CancellationToken ct = default)
    {
        await _dbContext.Tasks.Where(te => te.Id == taskId)
            .ExecuteDeleteAsync(ct);
    }
}