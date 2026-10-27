using Microsoft.AspNetCore.Rewrite;
using Telegram.Bot;
using Telegram.Bot.Types;
using music_time_manager.Application.Services;
using music_time_manager.Core.Result;

namespace music_time_manager.Infrastructure.Notifications;

public class TelegramUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ITelegramLinkService _linkService;

    public TelegramUpdateHandler(ITelegramBotClient botClient,
        ITelegramLinkService linkService)
    {
        _botClient = botClient;
        _linkService = linkService;
    }

    public async Task<Result> HandleUpdate(Update update, CancellationToken ct)
    {
        if(update.Message is not {} message)
            return Result.Success; // TODO: Maybe think of a better approach than returning Result.Success;
        
        var text = update.Message.Text;
        
        if(string.IsNullOrWhiteSpace(text))
            return Result.Success;

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
            return Result.Success;
        }
        
        var token = parts[1].Trim();

        var result = await _linkService.LinkAsync(
            token, 
            message.Chat.Id,
            ct);

        if (result.IsFailure)
        { 
            string responseFailure = result.Errors.First().Description;
            
            await _botClient.SendMessage(
                message.Chat.Id,
                responseFailure,
                cancellationToken: ct);
            
            return Result.Success;
        }
        
        const string responseSuccess = "Telegram is successfully linked to your account";

        await _botClient.SendMessage(
            message.Chat.Id,
            responseSuccess,
            cancellationToken: ct);
        
        return Result.Success;
    }
}