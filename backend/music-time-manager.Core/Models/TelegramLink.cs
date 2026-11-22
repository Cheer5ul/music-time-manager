using music_time_manager.Core.Result;

namespace music_time_manager.Core.Models;

public class TelegramLink
{
    public Guid UserId { get; private set; }
    public long ChatId { get; private set; }

    private TelegramLink(Guid userId, long chatId)
    {
        UserId = userId;
        ChatId = chatId;
    }

    public static ResultT<TelegramLink> Create(Guid userId, long chatId)
    {
        var telegramLink = new TelegramLink(userId, chatId);
        
        return ResultT<TelegramLink>.Success(telegramLink);
    }

    public static TelegramLink Reconstitute(Guid userId, long chatId)
    {
        return new TelegramLink(userId, chatId);
    }
}