namespace music_time_manager.Core.Errors.User;

public static class UserErrors
{
    private static class Codes
    {
        public const string InvalidUsername = "User.InvalidName";
    }
    public static Error InvalidUsername(string username) => 
        new Error(Codes.InvalidUsername,
            $"Username '{username}' is invalid."); 
}