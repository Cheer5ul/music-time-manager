namespace music_time_manager.Application.Services;

public interface ITelegramNotificationSender
{
    Task SendTextAsync(long chatId, string text, CancellationToken ct = default);
}