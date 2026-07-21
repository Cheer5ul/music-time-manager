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
        
        if (user == null) return ResultT<string>.Failures([UserErrors.NotFound(username)]);
        
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

        if (user == null) return ResultT<User>.Failures([UserErrors.NotFound(username)]);
        
        return ResultT<User>.Success(user);
    }

    public async Task<Result> Delete(Guid id, CancellationToken ct = default)
    {
        await _userRepository.Delete(id, ct);
        return Result.Success;
    }
}