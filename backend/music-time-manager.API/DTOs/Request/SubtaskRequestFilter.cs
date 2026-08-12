using CoreStatus = music_time_manager.Core.Models.Status;

namespace music_time_manager.Application.DTOs;

public record SubtaskRequestFilter(
    CoreStatus? Status,
    bool? IsOverdue,
    Guid? AssigneeId,
    Guid? TaskId
    );