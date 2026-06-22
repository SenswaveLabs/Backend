
using Senswave.DataSources.Application.Brokers.Brokers.Features.UpdateBroker;
using Senswave.DataSources.Domain.Brokers.Brokers.Entities;
using Senswave.DataSources.Domain.Brokers.Brokers.Repositories;
using Senswave.DataSources.Domain.Brokers.Clients.Proxy;
using Senswave.Integration.Homes.BrokerUsedInHome;

namespace Senswave.DataSources.Application.Brokers.Brokers.Features.DeleteBroker;

public class DeleteBrokerHandler(
    IClientProxy clienProxy,
    IBrokerCommandRepository repository,
    IRequestClient<BrokerUsageRequest> requestClient,
    ILogger<DeleteBrokerHandler> logger) : ICommandHandler<DeleteBrokerCommand>
{

    public async Task<Result> Handle(DeleteBrokerCommand request, CancellationToken cancellationToken)
    {
        var broker = await repository.GetBroker(request.BrokerId, request.UserId, cancellationToken);

        if (broker == null)
        {
            logger.LogWarning("[User: {userId}] Broker with id = {brokerId} not found.", request.UserId, request.BrokerId);
            return Result.Failure(DeleteBrokerConnectionErrors.BrokerNotFound);
        }

        // It is fine as long as one user can remove home and remove broker
        // else proper distributed transaction has to occurhere.
        var isUsed = await requestClient.GetResponse<BrokerUsageResponse>(new BrokerUsageRequest { BrokerId = request.BrokerId }, cancellationToken);

        if (isUsed.Message.UsedInHomes > 0)
        {
            logger.LogWarning("[User: {userId}] [Broker: {brokerId} ] Broker is used in {homes} homes.", request.UserId, request.BrokerId, isUsed.Message.UsedInHomes);
            return Result.Failure(DeleteBrokerConnectionErrors.BrokerIsUsed);
        }

        var exists = await clienProxy.ClientExists(request.BrokerId, cancellationToken);

        if (exists.IsSuccess)
        {
            logger.LogWarning("[User: {userId}] [Broker: {brokerId}] Broker has an active client.", request.UserId, request.BrokerId);
            return Result<Broker>.Failure(UpdateBrokerErrors.ClientExists);
        }

        var removeResult = await repository.DeleteBroker(broker, cancellationToken);

        if (removeResult.IsFailure)
        {
            logger.LogError("[User: {userId}] [Broker: {brokerId}] Failed to remove broker.", request.UserId, request.BrokerId);
            return Result.Failure(DeleteBrokerConnectionErrors.FailedToRemoveBroker);
        }

        logger.LogInformation("[User: {userId}] [Broker: {brokerId}] Broker was removed.", request.UserId, request.BrokerId);
        return Result.Success();
    }
}