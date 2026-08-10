namespace music_time_manager.API.DTOs;

public record UserStatsResponse(
    int CompletedTasks,
    int MissedTasks);