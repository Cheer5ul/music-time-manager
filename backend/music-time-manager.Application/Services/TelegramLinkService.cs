using Microsoft.Extensions.Options;
using music_time_manager.Core.Errors;
using music_time_manager.Core.Errors.User;
using music_time_manager.Core.Models;
using music_time_manager.Core.Result;
using music_time_manager.Persistence.Repositories;

namespace music_time_manager.Application.Services;

public class TelegramLinkService : ITelegramLinkService
{
    private readonly ITelegramRepository _tgRepository;
    private readonly ITelegramNotificationSender _tgNotificationSender;
    private readonly IUserRepository _userRepository;

    public TelegramLinkService(
        ITelegramRepository tgRepository,
        ITelegramNotificationSender tgNotificationSender,
        IUserRepository userRepository)
    {
        _tgRepository = tgRepository;
        _tgNotificationSender = tgNotificationSender;
        _userRepository = userRepository;
    }

    public async Task<ResultT<string>> CreateLinkToken(Guid userId, CancellationToken ct = default)
    {
        var doesUserExist = await _userRepository.GetById(userId, ct);
        if(doesUserExist == null) return ResultT<string>.Failures([UserErrors.DoesNotExist(userId)]);
        
        var checkTgLinkToken = await _tgRepository.GetLinkTokenByUserId(userId, ct);
        if (checkTgLinkToken != null && DateTime.UtcNow - checkTgLinkToken.ExpiresAt <
            TimeSpan.FromMinutes(TelegramLinkToken.LINK_TOKEN_EXPIRES_AFTER_MINUTES)) /* TODO:  AND CHAT WITH BOT NOT STARTED YET*/
        {
            return ResultT<string>.Failures([TelegramErrors.TelegramRegistrationLinkStillValid(
                _tgNotificationSender.BuildStartLink(checkTgLinkToken.Token))]);
        }
        
        if (checkTgLinkToken != null && DateTime.UtcNow - checkTgLinkToken.ExpiresAt >
            TimeSpan.FromMinutes(TelegramLinkToken.LINK_TOKEN_EXPIRES_AFTER_MINUTES))
        {
            var token = await UpdateLinkToken(checkTgLinkToken, ct); 
            if(token.IsFailure) return ResultT<string>.Failures(token.Errors);
            
            return ResultT<string>.Success(_tgNotificationSender.BuildStartLink(token.Value!));
        }
        
        var tgLinkToken = TelegramLinkToken.Create(userId);
        if(tgLinkToken.IsFailure) return ResultT<string>.Failures(tgLinkToken.Errors);
        
        await _tgRepository.CreateTelegramLinkToken(tgLinkToken.Value!, ct);

        var link = _tgNotificationSender.BuildStartLink(tgLinkToken.Value!.Token);
        
        return ResultT<string>.Success(link);
    }

    public async Task<Result> LinkAsync(string token, long chatId, CancellationToken ct = default)
    {
        var userId = await _tgRepository.GetUserIdByLinkToken(token, ct);
        if(userId == null) return Result.Failures([UserErrors.DoesNotExist()]);
        
        var doesLinkAlreadyExist = await _tgRepository.GetLinkByUserId(userId.Value, ct);
        if (doesLinkAlreadyExist is not null) return Result.Failures([TelegramErrors.TelegramAlreadyLinked()]);
        
        var linkTokenExpiresAt = await _tgRepository.GetLinkTokenExpiresAt(token, ct);
        if (linkTokenExpiresAt is null) return Result.Failures([TelegramErrors.LinkTokenNotFound()]);
        if (linkTokenExpiresAt.Value < DateTime.UtcNow)
            return Result.Failures([TelegramErrors.LinkTokenNotValidAnyMore()]);
        
        var tgLink = TelegramLink.Create(userId.Value, chatId);
        if(tgLink.IsFailure) return Result.Failures(tgLink.Errors);
        
        await _tgRepository.LinkTelegramAsync(tgLink.Value, ct);

        return Result.Success;
    }

    private async Task<ResultT<string>> UpdateLinkToken(TelegramLinkToken linkToken, CancellationToken ct)
    {
        var updateResult = linkToken.UpdateToken(linkToken);
        if (updateResult.IsFailure) return ResultT<string>.Failures(updateResult.Errors);
        
        await _tgRepository.UpdateLinkToken(linkToken.UserId, updateResult.Value!, ct);
        
        return ResultT<string>.Success(updateResult.Value!);
    }
}