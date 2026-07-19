namespace music_time_manager.API.DTOs;
using CoreStatus = music_time_manager.Core.Models.Status;

public record TaskResponse(
    Guid Id,
    string Title,
    string? Description,
    DateTime DueDate,
    DateTime CreatedAt,
    CoreStatus Status,
    bool IsOverdue,
    Guid CreatedBy,
    Guid? RecreatedFromTaskId
    // IReadOnlyList<UserSummary> Assignees,
    // IReadOnlyList<SubtaskResponse> Subtasks
    );