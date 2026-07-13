namespace music_time_manager.Persistence;

public class DbInitializer
{
    public static async Task InitializeAsync(MusicTimeManagerDbContext dbContext)
    {
        await dbContext.Database.EnsureCreatedAsync();
    }
}