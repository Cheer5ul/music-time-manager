namespace music_time_manager.Application.DTOs;

public record TaskRequest(
    string Title, DateTime DueDate,
    Guid CreatedBy,
    string? Description);
    
