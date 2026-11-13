using music_time_manager.Core.Result;

namespace music_time_manager.Application.Interfaces.Services;

public interface ITelegramLinkService
{
    Task<ResultT<string>> CreateLinkToken(Guid userId, CancellationToken ct = default);
    Task<Result> LinkAsync(string token, long chatId, CancellationToken ct = default);
    Task<ResultT<string>> UnlinkTgAccount(Guid userId, CancellationToken ct);
}