namespace music_time_manager.Core.Errors.User;

public static class UserErrors
{
    private static class Codes
    {
        public const string InvalidUsername = "User.InvalidName";
        public const string NotFoundName = "User.NotFoundName";
        public const string NotFound = "User.NotFound";
        public const string FailedToLogin = "User.FailedToLogin";
        public const string NameAlreadyUsed = "User.NameAlreadyUsed";
        public const string DoesNotExist = "User.DoesNotExist";
    }
    public static Error InvalidUsername(string username) => 
        new Error(Codes.InvalidUsername,
            $"Username '{username}' is invalid.",
            ErrorType.Validation); 
    public static Error NotFoundName(string username) => 
        new Error(Codes.NotFoundName,
            $"User with username '{username}' is not found.",
            ErrorType.NotFound);
    public static Error NotFound()
        => new Error(Codes.NotFound,
            $"User is not found.",
            ErrorType.NotFound);
    public static Error FailedToLogin() => 
        new Error(Codes.FailedToLogin,
            $"Failed to login.",
            ErrorType.Validation);
    public static Error NameAlreadyUsed(string name) =>
        new Error(Codes.NameAlreadyUsed,
            $"User with name '{name}' already exists.",
            ErrorType.Conflict);

    public static Error DoesNotExits(Guid id) =>
        new Error(Codes.DoesNotExist,
            $"User with id {id} does not exist.",
            ErrorType.NotFound);
}