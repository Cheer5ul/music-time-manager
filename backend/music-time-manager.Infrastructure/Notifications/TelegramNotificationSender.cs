using Microsoft.Extensions.Options;
using music_time_manager.Application.Services;
using music_time_manager.Infrastructure.Options;
using Telegram.Bot;

namespace music_time_manager.Infrastructure.Notifications;

public class TelegramNotificationSender : ITelegramNotificationSender
{
    private readonly ITelegramBotClient _botClient;
    private readonly TelegramOptions _tgOptions;

    public TelegramNotificationSender(
        ITelegramBotClient botClient,
        IOptions<TelegramOptions> options)
    {
        _botClient = botClient;
        _tgOptions = options.Value;
    }
    
    public async Task SendTextAsync(long chatId, string text, CancellationToken ct = default)
    {
        await _botClient.SendMessage(chatId, text, cancellationToken: ct); 
    }

    public string BuildStartLink(string token) 
        => $"https://t.me/{_tgOptions.BotUsername}?start={token}";
}