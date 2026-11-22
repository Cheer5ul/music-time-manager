using Microsoft.Extensions.Options;
using music_time_manager.Application.Interfaces.Services;
using music_time_manager.Application.Services;
using music_time_manager.Core.Errors;
using music_time_manager.Core.Interfaces;
using music_time_manager.Core.Result;
using music_time_manager.Infrastructure.Options;
using music_time_manager.Persistence.Repositories;
using Telegram.Bot;

namespace music_time_manager.Infrastructure.Notifications;

public class TelegramNotificationSender : ITelegramNotificationSender
{
    private readonly ITelegramBotClient _botClient;
    private readonly TelegramOptions _tgOptions;
    private readonly IUserRepository _userRepository;
    private readonly ITelegramRepository _telegramRepository;

    public TelegramNotificationSender(
        ITelegramBotClient botClient,
        IOptions<TelegramOptions> options,
        IUserRepository userRepository,
        ITelegramRepository telegramRepository)
    {
        _botClient = botClient;
        _tgOptions = options.Value;
        _userRepository = userRepository;
        _telegramRepository = telegramRepository;
    }
    
    public async Task SendTextAsync(long chatId, string text, CancellationToken ct = default)
    {
        await _botClient.SendMessage(chatId, text, cancellationToken: ct); 
    }

    public async Task<Result> NotifyUserAsync(Guid userId, CancellationToken ct = default)
    {
        var chatId = await _telegramRepository.GetChatIdByUserId(userId, ct);
        if (chatId is null)
            return ResultT<string>.Failures([
                TelegramErrors.UnableToNotifyUser(
                    _userRepository.GetById(userId, ct).Result.UserName)
            ]);

        string text = "You received a new task.";
        
        await _botClient.SendMessage(chatId, text, cancellationToken: ct);

        return Result.Success;
    }

    public string BuildStartLink(string token) 
        => $"https://t.me/{_tgOptions.BotUsername}?start={token}";
}