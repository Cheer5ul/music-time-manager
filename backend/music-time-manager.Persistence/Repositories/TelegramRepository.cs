using Microsoft.EntityFrameworkCore;
using music_time_manager.Core.Models;
using music_time_manager.Persistence.Entities;
using Task = System.Threading.Tasks.Task;

namespace music_time_manager.Persistence.Repositories;

public class TelegramRepository : ITelegramRepository
{
    private readonly MusicTimeManagerDbContext _dbContext;

    public TelegramRepository(MusicTimeManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TelegramLinkToken?> GetLinkTokenByUserId(Guid userId, CancellationToken ct)
    {
        var tgLinkTokenEntity = await _dbContext.TelegramLinkTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);
        
        if(tgLinkTokenEntity == null) return null;

        var tgLinkToken = TelegramLinkToken
            .Reconstitute(tgLinkTokenEntity.UserId, tgLinkTokenEntity.Token, tgLinkTokenEntity.ExpiresAt);
         return tgLinkToken;
    }

    public async Task<TelegramLink?> GetLinkByUserId(Guid userId, CancellationToken ct)
    {
        var tgLinkEntity = await _dbContext.TelegramLinks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);
        
        if(tgLinkEntity == null) return null;
        
        var tgLink = TelegramLink.Reconstitute(tgLinkEntity.UserId, tgLinkEntity.ChatId);
        return tgLink;
    }

    public async Task<Guid?> GetUserIdByLinkToken(string token, CancellationToken ct)
    {
        var userId = await _dbContext.TelegramLinkTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Token == token, ct);
        
        if(userId == null) return null;
        
        return userId.UserId;
    }

    public async Task<DateTime?> GetLinkTokenExpiresAt(string token, CancellationToken ct)
    {
        var dueDate = await _dbContext.TelegramLinkTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Token == token, ct);

        if (dueDate == null) return null;
        
        return dueDate.ExpiresAt;
        
    }
    
    public async Task CreateTelegramLinkToken(TelegramLinkToken tgLinkToken, CancellationToken ct)
    {
        var tgLinkTokenEntity = new TelegramLinkTokenEntity()
        {
            UserId = tgLinkToken.UserId,
            Token = tgLinkToken.Token,
            ExpiresAt = tgLinkToken.ExpiresAt
        };

        await _dbContext.TelegramLinkTokens.AddAsync(tgLinkTokenEntity, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task LinkTelegramAsync(TelegramLink telegramLink, CancellationToken ct)
    {
        var tgLinkEntity = new TelegramLinkEntity()
        {
            UserId = telegramLink.UserId,
            ChatId = telegramLink.ChatId
        };
        
        await _dbContext.TelegramLinks.AddAsync(tgLinkEntity, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateLinkToken(Guid userId, string newToken, CancellationToken ct)
    {
        await _dbContext.TelegramLinkTokens
            .Where(e => e.UserId == userId)
            .ExecuteUpdateAsync(
                s => s.SetProperty(e => e.Token, newToken)
                    .SetProperty(e => e.ExpiresAt, 
                        DateTime.UtcNow + TimeSpan.FromMinutes(TelegramLinkToken.LINK_TOKEN_EXPIRES_AFTER_MINUTES)),
                ct);
    }
}