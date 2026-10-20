using Telegram.Bot;
using Telegram.Bot.Types;

namespace music_time_manager.Infrastructure.Notifications;

public class TelegramUpdateHandler
{
    private readonly ITelegramBotClient _botClient;

    public TelegramUpdateHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task HandleUpdate(Update update, CancellationToken ct)
    {
        if(update.Message is not {} message)
            return;
        
        var text = update.Message.Text;
        
        if(string.IsNullOrWhiteSpace(text))
            return;

        var parts = text.Split(
            ' ',
            2,
            StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2 ||
            !string.Equals(
                parts[0].Split('@')[0],
                "/start",
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        
        var token = parts[1].Trim();

        // var result = await _linkService.LinkAsync();
        //
        //
    }
}