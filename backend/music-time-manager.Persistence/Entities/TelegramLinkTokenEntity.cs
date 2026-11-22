namespace music_time_manager.Persistence;

public class TelegramLinkTokenEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}