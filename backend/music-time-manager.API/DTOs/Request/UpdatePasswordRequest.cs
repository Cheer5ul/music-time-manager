namespace music_time_manager.Application.DTOs;

public record UpdatePasswordRequest(
    string CurrentPassword,
    string NewPassword);