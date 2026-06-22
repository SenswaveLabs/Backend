using Senswave.DataSources.Domain.Brokers.Clients.Proxy;
using Senswave.DataSources.Domain.Brokers.Sessions.Repositories;

namespace Senswave.DataSources.Application.Brokers.Clients.ClientState;

internal sealed class ClientStateHandler(
    ISessionQueryRepository repository,
    IClientProxy clientProxy,
    ILogger<ClientStateHandler> logger) : IQueryHandler<ClientStateQuery, ClientStateModel>
{
    public async Task<Result<ClientStateModel>> Handle(ClientStateQuery request, CancellationToken cancellationToken)
    {
        var broker = await repository.GetBroker(request.UserId, request.BrokerId, cancellationToken);

        if (broker is null)
        {
            logger.LogWarning("[User: {userId}] Broker with id = {brokerId} not found.", request.UserId, request.BrokerId);
            return Result<ClientStateModel>.Failure(ClientStateErrors.BrokerNotFound);
        }

        var state = await clientProxy.ClientState(request.BrokerId, cancellationToken);

        if (state.IsFailure)
        {
            logger.LogError("[User: {userId}] [Broker: {brokerId}] Failed to get client state.",
                request.UserId, request.BrokerId);
            return Result<ClientStateModel>.Failure(ClientStateErrors.FailedToGetStatus);
        }

        var response = new ClientStateModel
        {
            State = state.Data,
        };

        var latestSession = await repository.GetLatestSessionForBroker(request.BrokerId, request.UserId, cancellationToken);

        if (latestSession is not null && latestSession.Id != Guid.Empty)
            response.LatestSession = latestSession.Id;

        logger.LogInformation("[User: {userId}] [Broker: {brokerId}] Client state retrieved successfully.", request.UserId, request.BrokerId);
        return Result<ClientStateModel>.Success(response);
    }
}
