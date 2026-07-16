using music_time_manager.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace music_time_manager.Persistence.Repositories;

public interface IUserRepository
{
    Task Create(User user, CancellationToken ct);
    
    Task<User?> GetByUsername(string username, CancellationToken ct);
    Task Delete(Guid id, CancellationToken ct);
}