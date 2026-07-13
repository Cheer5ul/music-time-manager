using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace music_time_manager.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(nameof(MusicTimeManagerDbContext));

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string is empty or null");
        }

        services.AddDbContext<MusicTimeManagerDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString(nameof(MusicTimeManagerDbContext)));
        });

        // add services 
        // services.AddScoped<>();
        
        return services;
    }
}