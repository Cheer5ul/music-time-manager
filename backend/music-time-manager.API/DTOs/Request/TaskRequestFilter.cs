using CoreStatus = music_time_manager.Core.Models.Status;

namespace music_time_manager.Application.DTOs;

public record TaskRequestFilter(
    CoreStatus? Status,
    bool? IsOverdue,
    Guid? AssigneeId,
    Guid? CreatedBy,
    DateTime? DueBefore,
    DateTime? DueAfter,
    bool? HasAssignees
    );