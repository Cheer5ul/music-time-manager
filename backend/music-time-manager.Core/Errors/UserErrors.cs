namespace music_time_manager.Core.Errors.User;

public static class UserErrors
{
    private static class Codes
    {
        public const string InvalidUsername = "User.InvalidUserName";
        public const string FailedToLogin = "User.FailedToLogin";
        public const string UsernameAlreadyUsed = "User.UsernameAlreadyUsed";
        public const string DoesNotExist = "User.DoesNotExist";
        public const string InvalidPassword = "User.InvalidPassword";
        public const string IncorrectCurrentPassword = "User.IncorrectCurrentPassword";
    }
    public static Error InvalidUsername(string username) => 
        new Error(Codes.InvalidUsername,
            $"Username '{username}' is invalid.",
            ErrorType.Validation); 
    
    public static Error FailedToLogin() => 
        new Error(Codes.FailedToLogin,
            $"Failed to login.",
            ErrorType.Validation);
    
    public static Error NameAlreadyUsed(string name) =>
        new Error(Codes.UsernameAlreadyUsed,
            $"User with name '{name}' already exists.",
            ErrorType.Conflict);
    
    public static Error DoesNotExist() =>
        new Error(Codes.DoesNotExist,
            $"User does not exist.",
            ErrorType.NotFound);
    public static Error DoesNotExist(Guid id) =>
        new Error(Codes.DoesNotExist,
            $"User with id {id} does not exist.",
            ErrorType.NotFound);
    
    public static Error DoesNotExist(string username) =>
        new Error(Codes.DoesNotExist,
            $"User with username '{username}' does not exist.",
            ErrorType.NotFound);
    
    public static Error InvalidPassword() =>
        new Error(Codes.InvalidPassword,
            $"Password is invalid.",
            ErrorType.Validation);
    
    public static Error IncorrectCurrentPassword() =>
        new Error(Codes.IncorrectCurrentPassword,
            $"Current password is incorrect.",
            ErrorType.Validation);
}