using Senswave.Homes.Domain.Homes.Repositories;
using Senswave.Homes.Domain.Homes.Services;
using Senswave.Integration.DataSource.DeleteAllSubscribtions;
using Senswave.Integration.Devices.DevicesInHome;

namespace Senswave.Homes.Application.Homes.Features.DeleteHomeDataSource;

internal class DeleteHomeDataSourceHandler(
    IHomeAccessService accessService,
    IHomeCommandRepository repository,
    IRequestClient<DeleteAllSubscribtionsRequest> deleteAllSubscribtionsRequestClient,
    IRequestClient<DevicesInHomeRequest> devicesRequestClient,
    ILogger<DeleteHomeDataSourceHandler> logger) : ICommandHandler<DeleteHomeDataSourceCommand>
{
    public async Task<Result> Handle(DeleteHomeDataSourceCommand command, CancellationToken cancellationToken)
    {
        var isOwner = await accessService.IsOwner(command.UserId, command.HomeId, cancellationToken);

        if (!isOwner)
            return Result.Failure(isOwner.Errors);

        var home = await repository.GetHome(command.HomeId, cancellationToken);

        if (home is null)
        {
            logger.LogWarning("[User: {UserId}] [Home: {HomeId}] Home not found.", command.UserId, command.HomeId);
            return Result.Failure(DeleteHomeDataSourceErrors.HomeNotFound);
        }

        var devicesInHomeRequest = new DevicesInHomeRequest
        {
            HomeId = command.HomeId
        };

        var responseDevices = await devicesRequestClient.GetResponse<DevicesInHomeResponse>(devicesInHomeRequest, cancellationToken);

        if (responseDevices.Message.DevicesCount > 0)
        {
            logger.LogWarning("[User: {UserId}] [Home: {HomeId}] Home has devices assigned, cannot delete data source.", command.UserId, command.HomeId);
            return Result.Failure(DeleteHomeDataSourceErrors.DevicesExist);
        }

        if (home.DataSourceReference is not null)
        {
            var deleteAllSubscribtionsRequest = new DeleteAllSubscribtionsRequest
            {
                DataSourceId = home.DataSourceReference.DataSourceId
            };

            var removeSubscribtions = await deleteAllSubscribtionsRequestClient.GetResponse<DeleteAllSubscribtionsResponse>(deleteAllSubscribtionsRequest, cancellationToken);

            if (removeSubscribtions.Message.IsFailure)
            {
                logger.LogError("[User: {UserId}] [Home: {HomeId}] Failed to remove data source subscriptions.", command.UserId, command.HomeId);
                return Result.Failure(DeleteHomeDataSourceErrors.FailedToCleanSubscribtionsForDataSource);
            }
        }

        var result = await repository.DeleteDataSourceFromHome(home, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogError("[User: {UserId}] [Home: {HomeId}] Failed to delete data source from home.", command.UserId, command.HomeId);
            return Result.Failure(result.Errors);
        }

        logger.LogInformation("[User: {UserId}] [Home: {HomeId}] Data source deleted successfully.", command.UserId, command.HomeId);
        return Result.Success();
    }
}
