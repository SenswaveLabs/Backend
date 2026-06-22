using Senswave.Devices.Domain.Operations.Repositories;
using Senswave.Devices.Domain.Operations.Services;
using Senswave.Integration.DataSource.SubscribtionTopic;

namespace Senswave.Devices.Application.Operations.Features.GetOperation;

public class GetOperationHandler(
    IOperationAccessService accessService,
    IOperationQueryRepository repository,
    IRequestClient<SubscribtionTopicRequest> topicRequestClient,
    ILogger<GetOperationHandler> logger) : IQueryHandler<GetOperationQuery, ExtendedOperationModel>
{
    public async Task<Result<ExtendedOperationModel>> Handle(GetOperationQuery request, CancellationToken cancellationToken)
    {
        var canDisplay = await accessService.CanDisplay(request.UserId, request.OperationId, cancellationToken);

        if (!canDisplay)
            return Result<ExtendedOperationModel>.Failure(canDisplay.Errors);

        var operation = await repository.GetOperation(request.OperationId, cancellationToken);

        if (operation is null)
        {
            logger.LogWarning("[User: {UserId}] Operation not found: {OperationId}.",
                request.UserId,
                request.OperationId);
            return Result<ExtendedOperationModel>.Failure(GetOperationError.OperationNotFound);
        }

        // TODO: Home DataSource Migration

        var topicRequest = new SubscribtionTopicRequest
        {
            SubscriptionId = operation.DataReference!.DataSourceDataReferenceId
        };

        var topicResponse = await topicRequestClient.GetResponse<SubscribtionTopicResponse>(topicRequest, cancellationToken);

        if (topicResponse.Message.IsFailure)
        {
            logger.LogWarning("[User: {UserId}] Failed to retrieve topic for operation: {OperationId}.",
                request.UserId,
                request.OperationId);
            return Result<ExtendedOperationModel>.Failure(GetOperationError.TopicNotFound);
        }

        var extendedOperation = new ExtendedOperationModel
        {
            Operation = operation,
            Topic = topicResponse.Message.Topic
        };

        logger.LogInformation("[User: {UserId}] Retrieved operation details for operation: {OperationId}",
            request.UserId,
            request.OperationId);
        return Result<ExtendedOperationModel>.Success(extendedOperation);
    }
}