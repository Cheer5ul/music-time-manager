namespace music_time_manager.Persistence.Repositories;

public class UserRepository
{
    private readonly MusicTimeManagerDbContext _dbContext;
    public UserRepository(MusicTimeManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    
}