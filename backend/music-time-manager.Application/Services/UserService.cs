using music_time_manager.Application.DTOs;
using music_time_manager.Core.Errors.User;
using music_time_manager.Core.Models;
using music_time_manager.Core.Result;
using music_time_manager.Persistence.Repositories;

namespace music_time_manager.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> Create(CreateUserDto dto, CancellationToken ct = default)
    {
        var user = User.Create(dto.Name, dto.Password);
        
        if(user.IsFailure) return Result.Failures(user.Errors);
        
        await _userRepository.Create(user.Value!, ct);

        return Result.Success;
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