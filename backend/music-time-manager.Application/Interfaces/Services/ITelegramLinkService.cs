using music_time_manager.Core.Result;

namespace music_time_manager.Application.Services;

public interface ITelegramLinkService
{
    Task<ResultT<string>> CreateLink(Guid userId, CancellationToken ct = default);
}