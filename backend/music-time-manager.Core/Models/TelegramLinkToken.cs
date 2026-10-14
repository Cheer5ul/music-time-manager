using music_time_manager.Core.Errors;
using music_time_manager.Core.Result;

namespace music_time_manager.Core.Models;

public class TelegramLinkToken
{
    public const int LINK_TOKEN_EXPIRES_AFTER_MINUTES = 10;
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
        string token = GenerateToken(userId);

        DateTime expiresAt = DateTime.UtcNow + TimeSpan.FromMinutes(LINK_TOKEN_EXPIRES_AFTER_MINUTES);

        var tgLinkToken = new TelegramLinkToken(userId, token, expiresAt);
        return ResultT<TelegramLinkToken>.Success(tgLinkToken);
    }

    public static TelegramLinkToken Reconstitute(Guid userId, string token, DateTime expiresAt)
    {
        return new TelegramLinkToken(userId, token, expiresAt);
    }

    public ResultT<string> UpdateToken(TelegramLinkToken linkToken)
    {
        if (DateTime.UtcNow - linkToken.ExpiresAt < TimeSpan.FromMinutes(LINK_TOKEN_EXPIRES_AFTER_MINUTES))
        {
            return ResultT<string>.Failures([TelegramErrors.TokenStillValid()]);
        }
        
        var token = GenerateToken(linkToken.UserId);
        return ResultT<string>.Success(token);
    }
    
    private static string GenerateToken(Guid userId)
    {
        string token = string.Empty;
        Random random = new Random();
        int[] ints = Enumerable.Repeat(0, 4).Select(i => random.Next(0, 100)).ToArray();
        string firstFivChar = userId.ToString().Substring(0, 4);
        for (int i = 0; i < ints.Length; i++)
        {
            token = token + ints[i] + firstFivChar[i];
        }

        return token;
    }
}
