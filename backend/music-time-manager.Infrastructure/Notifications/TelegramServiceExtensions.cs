using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using music_time_manager.Application.Services;
using music_time_manager.Infrastructure.Options;
using Telegram.Bot;

namespace music_time_manager.Infrastructure.Notifications;

public static class TelegramServiceExtensions
{
    public static IServiceCollection AddTelegramService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TelegramOptions>(configuration.GetSection(nameof(TelegramOptions)));

        services.AddSingleton<ITelegramBotClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<TelegramOptions>>().Value;
            return new TelegramBotClient(options.BotToken);
        });

        services.AddScoped<ITelegramNotificationSender, TelegramNotificationSender>();
        
        return services;
    }
}