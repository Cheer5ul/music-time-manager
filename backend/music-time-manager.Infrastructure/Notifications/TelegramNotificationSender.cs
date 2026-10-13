using music_time_manager.Application.Services;
using Telegram.Bot;

namespace music_time_manager.Infrastructure.Notifications;

public class TelegramNotificationSender : ITelegramNotificationSender
{
    private readonly ITelegramBotClient _botClient;

    public TelegramNotificationSender(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }
    
    public async Task SendTextAsync(long chatId, string text, CancellationToken ct = default)
    {
        await _botClient.SendMessage(chatId, text, cancellationToken: ct); 
    }
}