namespace music_time_manager.Infrastructure.Options;

public class JwtOptions
{
    public string SecretKey { get; init; } = string.Empty;
    public int ExpiresHours { get; init; } 
}