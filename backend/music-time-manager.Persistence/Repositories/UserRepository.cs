using Microsoft.EntityFrameworkCore;
using music_time_manager.Core.Models;
using music_time_manager.Persistence.Entities;
using Task = System.Threading.Tasks.Task;

namespace music_time_manager.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MusicTimeManagerDbContext _dbContext;
    public UserRepository(MusicTimeManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<User>> GetUsers(CancellationToken ct)
    {
        var userEntities = await _dbContext.Users
            .AsNoTracking()
            .ToListAsync(ct);
        
        var users = userEntities.Select(u => 
            User.Reconstitute(u.Id, u.UserName, u.PasswordHash)).ToList();
        
        return users;
    }

    public async Task<User?> GetByUsername(string username, CancellationToken ct)
    {
        var userEntity = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == username, ct);
        
        if(userEntity == null) return null;
        
        var user = User.Reconstitute(userEntity.Id, userEntity.UserName, userEntity.PasswordHash);
        return user;
    }

    public async Task<User?> GetById(Guid id, CancellationToken ct)
    {
        var userEntity = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, ct);
        
        if(userEntity == null) return null;
        
        var user = User.Reconstitute(userEntity.Id, userEntity.UserName, userEntity.PasswordHash);
        return user;
    }

    public async Task<(int CompletedCount, int MissedCound)> GetStats(Guid userId, CancellationToken ct)
    {
        var completedTasksCount = await _dbContext.TaskAssignees
            .AsNoTracking()
            .Where(ta => ta.UserId == userId && ta.Task.Status == Status.Done)
            .CountAsync(ct);
        
        var completedSubtasksCount = await _dbContext.SubtaskAssignees
            .AsNoTracking()
            .Where(sa => sa.UserId == userId && sa.Subtask.Status == Status.Done)
            .CountAsync(ct);
        
        var missedTaskCount = await _dbContext.TaskAssignees
            .AsNoTracking()
            .Where(ta => ta.UserId == userId 
                         && ta.Task.Status != Status.Done
                         && ta.Task.DueDate < DateTime.UtcNow)
            .CountAsync(ct);
        
        var missedSubtaskCount = await _dbContext.SubtaskAssignees
            .AsNoTracking()
            .Where(sa => sa.UserId == userId 
                         && sa.Subtask.Status != Status.Done
                         && sa.Subtask.Task.DueDate < DateTime.UtcNow)
            .CountAsync(ct);
        
        return (completedTasksCount + completedSubtasksCount, missedTaskCount + missedSubtaskCount);
    }

    public async Task Create(User user, CancellationToken ct)
    {
        var userEntity = new UserEntity()
        {
            Id = user.Id,
            UserName = user.UserName,
            PasswordHash = user.PasswordHash,
        };
        
        await _dbContext.Users.AddAsync(userEntity, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateUsername(Guid id, string newUsername, CancellationToken ct)
    {
        await _dbContext.Users
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(
                s => s.SetProperty(u => u.UserName, newUsername),
                ct);
    }

    public async Task UpdatePassword(Guid id, string newPassword, CancellationToken ct)
    {
        await _dbContext.Users.Where(u => u.Id == id)
            .ExecuteUpdateAsync(
                s => s.SetProperty(u => u.PasswordHash, newPassword),
                ct);
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        await _dbContext.Users.Where(u => u.Id == id)
            .ExecuteDeleteAsync(ct);
    }
}