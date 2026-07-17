using System.ComponentModel.DataAnnotations;

namespace music_time_manager.Application.DTOs;

public record LoginUserRequest(
    [Required] string Username,
    [Required] string Password);