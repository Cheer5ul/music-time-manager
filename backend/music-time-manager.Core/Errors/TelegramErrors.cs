namespace music_time_manager.Core.Errors;

public static class TelegramErrors
{
    private static class Codes
    {
        public const string TokenStillValid = "Telegram.TokenStillValid";
        public const string TelegramRegistrationLinkStillValid = "Telegram.TelegramRegistrationLinkStillValid";
    }
    
    public static Error TokenStillValid() =>
        new Error(Codes.TokenStillValid,
            $"Token is still valid, there is no need to update it.",
            ErrorType.Conflict);

    public static Error TelegramRegistrationLinkStillValid(string link) =>
        new Error(Codes.TelegramRegistrationLinkStillValid,
            $"Your telegram registration link is still valid: {link}",
            ErrorType.Conflict);
}