using music_time_manager.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace music_time_manager.Persistence.Repositories;

public interface ITelegramRepository
{
    Task<TelegramLinkToken?> GetLinkTokenByUserId(Guid userId, CancellationToken ct);
    Task CreateTelegramLinkToken(TelegramLinkToken tgLinkToken, CancellationToken ct);
    Task UpdateLinkToken(TelegramLinkToken tgLinkToken, CancellationToken ct);
}