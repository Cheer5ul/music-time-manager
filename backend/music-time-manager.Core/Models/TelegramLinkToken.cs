using music_time_manager.Core.Result;

namespace music_time_manager.Core.Models;

public class TelegramLinkToken
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }

    private TelegramLinkToken(Guid userId, string token, DateTime expiresAt)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
    }

    public static ResultT<TelegramLinkToken> Create(Guid userId)
    {
        string token = string.Empty;
        Random random = new Random();
        int[] ints = Enumerable.Repeat(0, 4).Select(i => random.Next(0, 100)).ToArray();
        string firstFivChar = userId.ToString().Substring(0, 4);
        for (int i = 0; i < ints.Length; i++)
        {
            token = token + ints[i] + firstFivChar[i];
        }

        DateTime expiresAt = DateTime.UtcNow + TimeSpan.FromMinutes(10);

        var tgLinkToken = new TelegramLinkToken(userId, token, expiresAt);
        return ResultT<TelegramLinkToken>.Success(tgLinkToken);
    }
    
}
