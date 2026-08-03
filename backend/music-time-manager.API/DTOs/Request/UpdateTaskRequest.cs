namespace music_time_manager.Application.DTOs;

public record UpdateTaskRequest(
    string? Title,
    string? Description,
    DateTime? DueDate);