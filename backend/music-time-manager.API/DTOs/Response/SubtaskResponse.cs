using CoreStatus = music_time_manager.Core.Models.Status;
namespace music_time_manager.API.DTOs;

public record SubtaskResponse(
    Guid Id,
    string Title,
    CoreStatus Status,
    // bool IsOverdue,
    Guid TaskId);