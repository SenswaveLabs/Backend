using Senswave.Abstractions.Cryptography;
using Senswave.Homes.Domain;
using Senswave.Homes.Domain.Sharings.Entities;
using Senswave.Homes.Domain.Sharings.Repositories;

namespace Senswave.Homes.Application.Sharings.Features.AcceptSharing;

public class AcceptSharingHandler(
    IOptionsSnapshot<HomeModuleOptions> options,
    IHomeSharingQueryRepository queryRepository,
    IHomeSharingCommandRepository repository,
    IPasswordHashingService hashingService,
    ILogger<AcceptSharingHandler> logger) : ICommandHandler<AcceptSharingCommand>
{
    public async Task<Result> Handle(AcceptSharingCommand request, CancellationToken cancellationToken)
    {
        var invitations = await repository.GetInvitationsByUser(request.UserId, cancellationToken);

        if (invitations.Count == 0)
        {
            logger.LogWarning("[User: {UserId}] No active invitations.", request.UserId);
            return Result.Failure(AcceptSharingErrors.NoActiveInvitations);
        }

        foreach (var invitation in invitations)
        {
            var isValid = hashingService.VerifyPassword(invitation.Password, request.Password);

            if (isValid)
                return await AcceptInvitation(invitation, cancellationToken);
        }

        return Result.Failure(AcceptSharingErrors.InvitationNotFound);
    }

    private async Task<Result> AcceptInvitation(HomeSharingInvitation invitation, CancellationToken cancellationToken)
    {
        if (invitation.ExpirationTimeUtc < DateTime.UtcNow)
        {
            await repository.DeleteSharingInvitation(invitation, cancellationToken);
            logger.LogError("[Invitation: {InvitationId}] Invitation expired.", invitation.Id);
            return Result.Failure(AcceptSharingErrors.InvitationExpired);
        }

        //TODO: Redis Lock per User

        var currentUserInHome = await queryRepository.CountUsersByHome(invitation.HomeId, cancellationToken);

        if (options.Value.Limits.UsersPerHome <= currentUserInHome)
            return Result.Failure(AcceptSharingErrors.TooManyUsersInHome);

        var homeSharing = new HomeSharing
        {
            HomeId = invitation.HomeId,
            UserId = invitation.FriendId,
            SharingType = invitation.Type,

            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        var result = await repository.CreateHomeSharing(homeSharing, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogError("[User: {userId}] [Home: {homeId}] Failed to create home sharing.", invitation.FriendId, invitation.HomeId);
            return Result.Failure(result.Errors);
        }

        logger.LogInformation("[HomeSharing: {HomeSharingId}] Home sharing created successfully.", homeSharing.Id);
        return Result.Success();
    }
}