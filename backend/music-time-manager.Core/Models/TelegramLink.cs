using music_time_manager.Core.Result;

namespace music_time_manager.Core.Models;

public class TelegramLink
{
    public Guid UserId { get; private set; }
    public Guid ChatId { get; private set; }

    private TelegramLink(Guid userId, Guid chatId)
    {
        UserId = userId;
        ChatId = chatId;
    }

    public static ResultT<TelegramLink> Create(Guid userId, Guid chatId)
    {
        var telegramLink = new TelegramLink(userId, chatId);
        
        return ResultT<TelegramLink>.Success(telegramLink);
    }

    public static TelegramLink Reconstitute(Guid userId, Guid chatId)
    {
        return new TelegramLink(userId, chatId);
    }
}