namespace music_time_manager.Application.DTOs;

public record AssigneesUpdateRequest(
    List<Guid> UserIds);