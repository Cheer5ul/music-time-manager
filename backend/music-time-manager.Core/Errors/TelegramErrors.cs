namespace music_time_manager.Core.Errors;

public static class TelegramErrors
{
    private static class Codes
    {
        public const string TokenStillValid = "Telegram.TokenStillValid";
        public const string TelegramRegistrationLinkStillValid = "Telegram.TelegramRegistrationLinkStillValid";
        public const string LinkTokenNotFound = "Telegram.LinkTokenNotFound";
        public const string LinkTokenNotValidAnyMore = "Telegram.TokenNotValidAnyMore";
        public const string TelegramAlreadyLinked = "Telegram.TelegramAlreadyLinked";
    }
    
    public static Error TokenStillValid() =>
        new Error(Codes.TokenStillValid,
            $"Token is still valid, there is no need to update it.",
            ErrorType.Conflict);

    public static Error TelegramRegistrationLinkStillValid(string link) =>
        new Error(Codes.TelegramRegistrationLinkStillValid,
            $"Your telegram registration link is still valid: {link}",
            ErrorType.Conflict);

    public static Error LinkTokenNotFound() =>
        new Error(Codes.LinkTokenNotFound,
            $"Telegram link token is not found.",
            ErrorType.NotFound);

    public static Error LinkTokenNotValidAnyMore() =>
        new Error(Codes.LinkTokenNotValidAnyMore,
            $"Telegram link token is not valid any more, please update it.",
            ErrorType.DomainInvariantViolation);

    public static Error TelegramAlreadyLinked() =>
        new Error(Codes.TelegramAlreadyLinked,
            $"Your telegram account has already been linked.",
            ErrorType.Conflict);
}