using System.ComponentModel.DataAnnotations;

namespace music_time_manager.Application.DTOs;

public record TaskRecreateRequest(
    [Required] DateTime DueDate,
    [MaxLength(200)] string? Title, 
    [MaxLength(2000)] string? Description,
    [MinLength(1)] List<Guid>? AssigneeIds
    );