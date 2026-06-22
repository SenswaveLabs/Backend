using MassTransit;
using Senswave.Integration.Automations.Remove;
using Senswave.Integration.DataSource.Remove;
using Senswave.Integration.Devices.Remove;
using Senswave.Integration.Homes.Remove;
using Senswave.Users.Domain.Entity;
using Senswave.Users.Domain.Enums;
using Senswave.Users.Domain.Repositories;

namespace Senswave.Users.Application.Users.DeleteAccount;

internal sealed class DeleteAccountHandler(
    IRequestClient<AutomationsRemoveRequest> automationsRemoveClient,
    IRequestClient<DataSourcesRemoveRequest> dataSourcesRemoveClient,
    IRequestClient<HomesRemoveRequest> homesRemoveClient,
    IRequestClient<DevicesRemoveRequest> devicesRemoveClient,
    ICommandUserRepository repository,
    ILogger<DeleteAccountHandler> logger) : ICommandHandler<DeleteAccountCommand>
{
    public async Task<Result> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.GetUser(request.UserId, cancellationToken);

        if (user is null)
        {
            logger.LogError("[User: {userId}] User not found for account deletion.", request.UserId);
            return Result.Failure(Error.NotFound("UserNotFound", "The user account to be deleted was not found."));
        }

        var automationsRequest = new AutomationsRemoveRequest { UserId = request.UserId };
        var automationRemoved = await automationsRemoveClient
            .GetResponse<AutomationsRemoveResponse>(automationsRequest, cancellationToken);

        if (automationRemoved.Message.IsFailure)
        {
            logger.LogError("[User: {userId}] Failed to remove user automations during account deletion.", request.UserId);
            return Result.Failure(automationRemoved.Message.Error);
        }

        var deviceRemove = await devicesRemoveClient
            .GetResponse<DevicesRemoveResponse>(new DevicesRemoveRequest { UserId = request.UserId }, cancellationToken);

        if (deviceRemove.Message.IsFailure)
        {
            logger.LogError("[User: {userId}] Failed to remove user devices during account deletion.", request.UserId);
            return Result.Failure(deviceRemove.Message.Error);
        }

        var homeRemove = await homesRemoveClient
            .GetResponse<HomesRemoveResponse>(new HomesRemoveRequest { UserId = request.UserId }, cancellationToken);

        if (homeRemove.Message.IsFailure)
        {
            logger.LogError("[User: {userId}] Failed to remove user homes during account deletion.", request.UserId);
            return Result.Failure(homeRemove.Message.Error);
        }

        var dataSourceRemove = await dataSourcesRemoveClient
            .GetResponse<DataSourcesRemoveResponse>(new DataSourcesRemoveRequest { UserId = request.UserId }, cancellationToken);

        if (dataSourceRemove.Message.IsFailure)
        {
            logger.LogError("[User: {userId}] Failed to remove user data sources during account deletion.", request.UserId);
            return Result.Failure(dataSourceRemove.Message.Error);
        }

        logger.LogInformation("[User: {userId}] User data removed successfully from modules.", request.UserId);

        var userForRemoval = new RemovedUser
        {
            Reason = request.Reason,
            SecurityStamp = string.Empty,
            HashedEmail = user.Email!,
            Status = UserRemovalStatus.UserDataRemoved
        };

        var result = await repository.RemoveUser(user, userForRemoval, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogError("[User: {userId}] Failed to delete user account. Errors: {Errors}",
                request.UserId, result.Errors.FirstOrDefault()?.Description);

            return Result.Failure(Error.ServerError("UserDeletionFailed", "An error occurred while deleting the user account."));
        }

        logger.LogInformation("[User: {userId}] User account deleted successfully.", request.UserId);
        return Result.Success();
    }
}
