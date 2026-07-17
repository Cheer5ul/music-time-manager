using System.ComponentModel.DataAnnotations;

namespace music_time_manager.Application.DTOs;

public record RegisterUserRequest(
    [Required] string Name,
    [Required] string Password); 