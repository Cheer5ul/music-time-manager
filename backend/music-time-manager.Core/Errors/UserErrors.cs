namespace music_time_manager.Core.Errors.User;

public static class UserErrors
{
    private static class Codes
    {
        public const string InvalidUsername = "User.InvalidName";
        public const string NotFound = "User.NotFound";
        public const string FailedToLogin = "User.FailedToLogin";
    }
    public static Error InvalidUsername(string username) => 
        new Error(Codes.InvalidUsername,
            $"Username '{username}' is invalid."); 
    public static Error NotFound(string username) => 
        new Error(Codes.NotFound,
            $"User with username '{username}' is not found.");
    public static Error FailedToLogin() => 
        new Error(Codes.FailedToLogin,
            $"Failed to login.");
}