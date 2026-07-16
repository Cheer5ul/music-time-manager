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

    public async Task<User?> GetByUsername(string username, CancellationToken ct)
    {
        var userEntity = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == username, ct);
        
        if(userEntity == null) return null;
        
        var user = User.Reconstitute(userEntity.Id, userEntity.UserName, userEntity.PasswordHash);
        return user;
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        await _dbContext.Users.Where(u => u.Id == id)
            .ExecuteDeleteAsync(ct);
    }
}