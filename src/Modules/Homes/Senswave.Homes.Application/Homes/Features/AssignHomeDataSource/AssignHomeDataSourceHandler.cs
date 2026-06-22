using Senswave.Homes.Domain.Homes.Repositories;
using Senswave.Homes.Domain.Homes.Services;
using Senswave.Integration.DataSource.BrokerAccess;
using Senswave.Integration.Devices.DevicesInHome;

namespace Senswave.Homes.Application.Homes.Features.AssignHomeDataSource;

internal class AssignHomeDataSourceHandler(
    IHomeAccessService accessService,
    IHomeCommandRepository repository,
    IRequestClient<BrokerAccessRequest> requestClient,
    IRequestClient<DevicesInHomeRequest> devicesRequestClient,
    ILogger<AssignHomeDataSourceHandler> logger) : ICommandHandler<AssignHomeDataSourceCommand>
{
    public async Task<Result> Handle(AssignHomeDataSourceCommand command, CancellationToken cancellationToken)
    {
        var isOwner = await accessService.IsOwner(command.UserId, command.HomeId, cancellationToken);

        if (!isOwner)
            return Result.Failure(isOwner.Errors);

        var home = await repository.GetHome(command.HomeId, cancellationToken);

        if (home is null)
        {
            logger.LogWarning("[User: {UserId}] [Home: {HomeId}] Home not found.", command.UserId, command.HomeId);
            return Result.Failure(AssignHomeDataSourceErrors.HomeNotFound);
        }

        if (home.DataSourceReference is not null)
        {
            logger.LogInformation("[User: {UserId}] [Home: {HomeId}] Home already has a data source assigned.", command.UserId, command.HomeId);
            return Result.Failure(AssignHomeDataSourceErrors.DataSourceAlreadyAssigned);
        }

        var request = new BrokerAccessRequest
        {
            BrokerId = command.DataSourceId,
            UserId = command.UserId
        };

        var response = await requestClient.GetResponse<BrokerAccessResponse>(request);

        if (response.Message.IsFailure)
        {
            logger.LogError("[User: {UserId}] [Home: {HomeId}] Failed to access data source.", command.UserId, command.HomeId);
            return Result.Failure(AssignHomeDataSourceErrors.DataSourceIsNotOwned);
        }

        var dataSourceUsed = await repository.CountHomeByDataSourceId(command.DataSourceId, cancellationToken);

        if (dataSourceUsed > 0)
        {
            logger.LogWarning("[User: {UserId}] [Home: {HomeId}] Data source is already assigned to another home.", command.UserId, command.HomeId);
            return Result.Failure(AssignHomeDataSourceErrors.DataSourceAlreadyUsed);
        }

        var responseDevices = await devicesRequestClient.GetResponse<DevicesInHomeResponse>(new DevicesInHomeRequest
        {
            HomeId = command.HomeId
        });

        if (responseDevices.Message.DevicesCount > 0)
        {
            logger.LogWarning("[User: {UserId}] [Home: {HomeId}] Home has devices assigned, cannot assign data source.", command.UserId, command.HomeId);
            return Result.Failure(AssignHomeDataSourceErrors.DevicesExist);
        }

        var result = await repository.AssignDataSourceToHome(home, command.DataSourceId, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogError("[User: {UserId}] [Home: {HomeId}] Failed to assign data source to home.", command.UserId, command.HomeId);
            return Result.Failure(result.Errors);
        }

        logger.LogInformation("[User: {UserId}] [Home: {HomeId}] Data source assigned successfully.", command.UserId, command.HomeId);
        return Result.Success();
    }
}
