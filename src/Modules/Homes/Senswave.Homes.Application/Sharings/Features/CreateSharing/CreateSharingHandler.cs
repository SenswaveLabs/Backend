using Senswave.Abstractions.Cryptography;
using Senswave.Homes.Domain;
using Senswave.Homes.Domain.Homes.Services;
using Senswave.Homes.Domain.Sharings.Entities;
using Senswave.Homes.Domain.Sharings.Repositories;
using Senswave.Homes.Domain.Sharings.Services;
using Senswave.Integration.User;

namespace Senswave.Homes.Application.Sharings.Features.CreateSharing;

public class CreateSharingHandler(
    IOptionsSnapshot<HomeModuleOptions> options,
    IHomeAccessService accessService,
    IRequestClient<UserByEmailRequest> userClient,
    IHomeSharingQueryRepository queryRepository,
    IHomeSharingCommandRepository commandRepository,
    ISharingPasswordGeneratorService passwordGeneratorService,
    IPasswordHashingService hashingService,
    ILogger<CreateSharingHandler> logger) : ICommandHandler<CreateSharingCommand, CreateSharingDto>
{
    public async Task<Result<CreateSharingDto>> Handle(CreateSharingCommand request, CancellationToken cancellationToken)
    {
        var isOwner = await accessService.IsOwner(request.OwnerId, request.HomeId, cancellationToken);

        if (!isOwner)
            return Result<CreateSharingDto>.Failure(isOwner.Errors);

        // TODO: Max invitations check yes there is a limit for users in home but invitations can be unlimited what can make a problem

        var userRequest = new UserByEmailRequest
        {
            Email = request.FriendEmail
        };

        var response = await userClient.GetResponse<UserByEmailResponse>(userRequest, cancellationToken);

        if (response.Message.IsFailure || response.Message.UserId == default)
        {
            logger.LogError("User for sharing not found by email.");
            return Result<CreateSharingDto>.Failure(CreateSharingErrors.UserNotFound);
        }

        if (response.Message.UserId == request.OwnerId)
        {
            logger.LogWarning("[User: {UserId}] User is the owner.", response.Message.UserId);
            return Result<CreateSharingDto>.Failure(CreateSharingErrors.UserIsOwner);
        }

        var invitationAlreadyExists = await queryRepository.GetInvitation(
            response.Message.UserId,
            request.HomeId,
            cancellationToken);

        if (invitationAlreadyExists is not null)
        {
            var removeResult = await commandRepository.DeleteSharingInvitation(invitationAlreadyExists, cancellationToken);

            if (removeResult.IsFailure)
            {
                logger.LogError("[User: {UserId}] Failed to remove existing invitation.", response.Message.UserId);
                return Result<CreateSharingDto>.Failure(removeResult.Errors);
            }
        }

        var expirationSeconds = options.Value.Sharings.InvitationExpiresInSeconds;

        var password = passwordGeneratorService.GeneratePassword();
        var hashedPassword = hashingService.HashPassword(password);

        var invitation = new HomeSharingInvitation
        {
            FriendId = response.Message.UserId,
            HomeId = request.HomeId,
            Password = hashedPassword,
            Type = request.SharingType,
            ExpirationTimeUtc = DateTime.UtcNow.AddSeconds(expirationSeconds),

            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        var result = await commandRepository.CreateHomeSharingInvitation(invitation, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogError("[User: {UserId}] [Home: {HomeId}] Failed to create home sharing invitation.",
                response.Message.UserId,
                request.HomeId);

            return Result<CreateSharingDto>.Failure(result.Errors);
        }

        var dto = new CreateSharingDto()
        {
            Id = invitation.Id,
            Password = password,
            CreatedAtUtc = invitation.CreatedAtUtc,
            ExpiresAtUtc = invitation.ExpirationTimeUtc
        };

        logger.LogInformation("[Invitation: {InvitationId}] Home sharing invitation created successfully.",
            dto.Id);

        return Result<CreateSharingDto>.Success(dto);
    }
}