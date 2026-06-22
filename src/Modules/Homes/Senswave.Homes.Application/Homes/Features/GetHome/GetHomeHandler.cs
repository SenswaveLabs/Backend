using Senswave.Homes.Application.Homes.Models;
using Senswave.Homes.Domain.Homes.Repositories;
using Senswave.Homes.Domain.Homes.Services;
using Senswave.Integration.DataSource.State;

namespace Senswave.Homes.Application.Homes.Features.GetHome;

public class GetHomeHandler(
    ILogger<GetHomeHandler> logger,
    IRequestClient<DataSourceStateRequest> client,
    IHomeAccessService accessService,
    IHomeQueryRepository repository) : IQueryHandler<GetHomeQuery, GetHomeModel>
{
    public async Task<Result<GetHomeModel>> Handle(GetHomeQuery request, CancellationToken cancellationToken)
    {
        var canDisplay = await accessService.CanDisplay(request.UserId, request.HomeId, cancellationToken);

        if (!canDisplay)
            return Result<GetHomeModel>.Failure(canDisplay.Errors);

        var home = await repository.GetHomeWithRooms(request.HomeId, cancellationToken);

        if (home is null)
        {
            logger.LogWarning("[User: {UserId}] [Home: {HomeId}] Home not found.", request.UserId, request.HomeId);
            return Result<GetHomeModel>.Failure(GetHomeErrors.HomeNotFound);
        }

        var model = new GetHomeModel
        {
            Home = home,
        };

        if (home.DataSourceReference is not null)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(7500);

            try
            {
                var internalRequest = new DataSourceStateRequest
                {
                    DataSourceReferenceId = home.DataSourceReference!.DataSourceId
                };

                var homeStateResult = await client.GetResponse<DataSourceStateResponse>(internalRequest, cts.Token);

                model.DataSource = new()
                {
                    Id = home.DataSourceReference!.DataSourceId,
                    Name = homeStateResult.Message.Name,
                    State = homeStateResult.Message.State
                };
            }
            catch (OperationCanceledException ex)
            {
                logger.LogError(ex, "[Home: {homeId}] Failed to retrive data source state. Operation timed out.", request.HomeId);

                model.DataSource = new()
                {
                    Id = home.DataSourceReference!.DataSourceId,
                    State = "Empty"
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[Home: {homeId}] Failed to retrive data source state.", request.HomeId);
                model.DataSource = new()
                {
                    Id = home.DataSourceReference!.DataSourceId,
                    State = "Empty"
                };
            }
        }

        logger.LogInformation("[User: {UserId}] Retrieved home {HomeId} details.", request.UserId, request.HomeId);
        return Result<GetHomeModel>.Success(model);
    }
}