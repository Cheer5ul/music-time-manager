using Microsoft.EntityFrameworkCore;
using music_time_manager.Core.Models;
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

    public async Task UpdateLinkToken(TelegramLinkToken tgLinkToken, CancellationToken ct)
    {
        await _dbContext.TelegramLinkTokens
            .Where(e => e.UserId == tgLinkToken.UserId)
            .ExecuteUpdateAsync(
                s => s.SetProperty(e => e.Token, tgLinkToken.Token),
                ct);
    }
}