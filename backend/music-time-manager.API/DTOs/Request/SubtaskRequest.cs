namespace music_time_manager.Application.DTOs;

public record SubtaskRequest(
    string Title, Guid TaskId);