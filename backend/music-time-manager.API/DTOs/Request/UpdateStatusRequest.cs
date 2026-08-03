using CoreStatus = music_time_manager.Core.Models.Status;
namespace music_time_manager.Application.DTOs;


public record UpdateStatusRequest(
    CoreStatus Status);