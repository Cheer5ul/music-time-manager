namespace music_time_manager.Persistence.Entities;

public class TelegramLinkEntity
{
    public Guid UserId { get; set; }
    public long ChatId { get; set; }
}