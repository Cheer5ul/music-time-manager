using music_time_manager.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace music_time_manager.Persistence.Repositories;

public interface ITelegramRepository
{
    Task<TelegramLinkToken?> GetLinkTokenByUserId(Guid userId, CancellationToken ct);
    Task<Guid?> GetUserIdByLinkToken(string token, CancellationToken ct);
    Task CreateTelegramLinkToken(TelegramLinkToken tgLinkToken, CancellationToken ct);
    Task CreateTelegramLink(TelegramLink telegramLink, CancellationToken ct);
    Task UpdateLinkToken(Guid userId, string newToken, CancellationToken ct);
}