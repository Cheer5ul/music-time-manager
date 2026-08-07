using music_time_manager.Core.Errors.User;
using music_time_manager.Core.Models;
using music_time_manager.Core.Result;
using music_time_manager.Infrastructure;
using music_time_manager.Persistence.Repositories;

namespace music_time_manager.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    
    public UserService(IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<ResultT<List<User>>> GetUsers(CancellationToken ct)
    {
        var users = await _userRepository.GetUsers(ct);

        return ResultT<List<User>>.Success(users);
    }

    public async Task<Result> Create(string username, string password, CancellationToken ct = default)
    {
        var isNameUsed = await _userRepository.GetByUsername(username, ct);
        if (isNameUsed != null) return Result.Failures([UserErrors.NameAlreadyUsed(username)]);
        
        var passwordHash = _passwordHasher.Generate(password);
        
        var user = User.Create(username, passwordHash);
        
        if(user.IsFailure) return Result.Failures(user.Errors);
        
        await _userRepository.Create(user.Value!, ct);

        return Result.Success;
    }

    public async Task<ResultT<string>> Login(string username, string password, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByUsername(username, ct);
        
        if (user == null) return ResultT<string>.Failures([UserErrors.NotFoundName(username)]);
        
        var result = _passwordHasher.Verify(password, user.PasswordHash);

        if (result == false)
        {
            return ResultT<string>.Failures([UserErrors.FailedToLogin()]);
        }

        var token = _jwtProvider.GenerateToken(user);
        
        return ResultT<string>.Success(token);
    }

    public async Task<ResultT<User>> GetByUsername(string username, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByUsername(username, ct);

        if (user == null) return ResultT<User>.Failures([UserErrors.NotFoundName(username)]);
        
        return ResultT<User>.Success(user);
    }

    public async Task<ResultT<User>> GetById(Guid id, CancellationToken ct = default)
    {
        var user = await _userRepository.GetById(id, ct);
        
        if(user == null) return ResultT<User>.Failures([UserErrors.NotFound()]);
        
        return ResultT<User>.Success(user);
    }

    public async Task<Result> UpdateUsername(Guid id, string newUsername, CancellationToken ct = default)
    {
        var result = User.UpdateUsername(newUsername);
        if(result.IsFailure) return Result.Failures(result.Errors);
        
        var doesUserExist = await _userRepository.GetById(id, ct);
        if (doesUserExist is null) return Result.Failures([UserErrors.DoesNotExits(id)]);
        
        await _userRepository.UpdateUsername(id, newUsername, ct);
        return Result.Success;
    }
    
    public async Task<Result> Delete(Guid id, CancellationToken ct = default)
    {
        await _userRepository.Delete(id, ct);
        return Result.Success;
    }
}