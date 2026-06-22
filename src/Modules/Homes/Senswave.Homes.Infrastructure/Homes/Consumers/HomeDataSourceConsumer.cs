using Senswave.Homes.Domain.Homes.Repositories;
using Senswave.Integration.Homes.HomeDataSource;
using Senswave.Integration.Shared;

namespace Senswave.Homes.Infrastructure.Homes.Consumers;

internal sealed class HomeDataSourceConsumer(
    IHomeQueryRepository repository,
    ILogger<HomeDataSourceConsumer> logger) : IConsumer<HomeDataSourceRequest>
{
    public async Task Consume(ConsumeContext<HomeDataSourceRequest> context)
    {
        var home = await repository.GetHome(context.Message.HomeId, context.CancellationToken);

        if (home is null)
        {
            var response = new HomeDataSourceResponse
            {
                StatusCode = InternalRequestStatus.Failure,
                Error = Error.NotFound("HomeNotFound", "Home not found.")
            };

            logger.LogWarning("[Home: {homeId}] Home not found.", context.Message.HomeId);
            await context.RespondAsync(response);
            return;
        }

        if (home.DataSourceReference is null)
        {
            var response = new HomeDataSourceResponse
            {
                StatusCode = InternalRequestStatus.Failure,
                Error = Error.Conflict("DataSourceNotSet", "No data source is assigned to this home.")
            };

            logger.LogWarning("[Home: {homeId}] Home does not have a data source set.", context.Message.HomeId);
            await context.RespondAsync(response);
            return;
        }

        var successResponse = new HomeDataSourceResponse
        {
            DataSourceId = home.DataSourceReference.DataSourceId,
            StatusCode = InternalRequestStatus.Success
        };

        logger.LogInformation("[Home: {homeId}] Home data source found with ID: {dataSourceId}.",
            context.Message.HomeId, home.DataSourceReference.DataSourceId);
        await context.RespondAsync(successResponse);
    }
}