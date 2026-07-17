using music_time_manager.Core.Models;

namespace music_time_manager.Infrastructure;

public interface IJwtProvider
{
    string GenerateToken(User user);
}