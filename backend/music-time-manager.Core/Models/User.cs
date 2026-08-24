using music_time_manager.Core.Errors.User;
using music_time_manager.Core.Result;

namespace music_time_manager.Core.Models;

public class User
{
    public const int MAX_USERNAME_LENGTH = 100;
    public const int MIN_PASSWORD_LENGTH = 3;
    private User(Guid id, string username, string passwordHash)
    {
        Id = id;
        UserName = username;
        PasswordHash = passwordHash;
    }
    public Guid Id { get; private set; }
    public string UserName  { get; private set; }
    public string PasswordHash {get; private set;}
    
    public static ResultT<User> Create(string username, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username) || username.Length > MAX_USERNAME_LENGTH)
        {
            return ResultT<User>.Failures([UserErrors.InvalidUsername(username)]);
        }
        var user = new User(
            id: Guid.NewGuid(),
            username: username,
            passwordHash: passwordHash);
        
        return ResultT<User>.Success(user);
    }

    public static User Reconstitute(Guid id, string username, string passwordHash)
    {
        return new User(id, username, passwordHash);
    }

    public static Result.Result UpdateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username) || username.Length > MAX_USERNAME_LENGTH)
        {
            return Result.Result.Failures([UserErrors.InvalidUsername(username)]);
        }
        return Result.Result.Success;
    }

    public static Result.Result ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < MIN_PASSWORD_LENGTH)
        {
            return Result.Result.Failures([UserErrors.InvalidPassword()]);
        }
        return Result.Result.Success;
    }
} 