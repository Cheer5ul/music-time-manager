using music_time_manager.Application.DTOs;
using music_time_manager.Core.Models;
using music_time_manager.Core.Result;

namespace music_time_manager.Application.Services;

public interface IUserService
{
    Task<Result> Create(CreateUserDto dto, CancellationToken ct = default);
    Task<ResultT<User>> GetByUsername(string username, CancellationToken ct = default);
    Task<Result> Delete(Guid id, CancellationToken ct = default);
}