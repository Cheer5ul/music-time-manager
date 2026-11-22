using music_time_manager.Core.Result;

namespace music_time_manager.Application.Interfaces.Services;

public interface ITelegramNotificationSender
{
    Task SendTextAsync(long chatId, string text, CancellationToken ct = default);
    Task<Result> NotifyUserAsync(Guid userId, CancellationToken ct = default);
    string BuildStartLink(string token);
}