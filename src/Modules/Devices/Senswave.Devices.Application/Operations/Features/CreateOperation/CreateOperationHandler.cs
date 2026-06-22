using Senswave.Devices.Domain.Devices.Options;
using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Factory;
using Senswave.Devices.Domain.Operations.Repositories;
using Senswave.Devices.Domain.Operations.Services;
using Senswave.Integration.DataSource.CreateSubscription;
using Senswave.Integration.Homes.HomeDataSource;

namespace Senswave.Devices.Application.Operations.Features.CreateOperation;

public class CreateOperationHandler(
    OperationFactory factory,
    IOperationAccessService accessService,
    IOperationQueryRepository queryRepository,
    IOperationCommandRepository commandRepository,
    IOptions<DevicesOptions> options,
    IRequestClient<HomeDataSourceRequest> defaultBrokerClient,
    IRequestClient<CreateSubscriptionRequest> subscriptionClient,
    ILogger<CreateOperationHandler> logger) : ICommandHandler<CreateOperationCommand, Operation>
{

    public async Task<Result<Operation>> Handle(CreateOperationCommand request, CancellationToken cancellationToken)
    {
        var canManage = await accessService.CanManageDevice(request.UserId, request.DeviceId, cancellationToken);

        if (!canManage)
            return Result<Operation>.Failure(CreateOperationErrors.NoAccess);

        var device = await commandRepository.GetDevice(request.DeviceId, cancellationToken);

        if (device is null)
        {
            logger.LogWarning("[User: {UserId}] Device not found: {DeviceId}.", request.UserId, request.DeviceId);
            return Result<Operation>.Failure(CreateOperationErrors.DeviceNotFound);
        }

        var dataSourceRequest = new HomeDataSourceRequest
        {
            HomeId = device.HomeReference.HomeId
        };

        var defaultBrokerResponse = await defaultBrokerClient
            .GetResponse<HomeDataSourceResponse>(dataSourceRequest, cancellationToken);

        if (defaultBrokerResponse.Message.IsFailure)
        {
            logger.LogWarning("[User: {UserId}] Default broker not found for home: {HomeId}.", request.UserId, device.HomeReference.HomeId);
            return Result<Operation>.Failure(CreateOperationErrors.BrokerNotFound);
        }

        var topicRequest = new CreateSubscriptionRequest
        {
            BrokerId = defaultBrokerResponse.Message.DataSourceId,
            Topic = request.Topic
        };

        var subscriptionResponse = await subscriptionClient
            .GetResponse<CreateSubscriptionResponse>(topicRequest, cancellationToken);

        if (subscriptionResponse.Message.IsFailure)
        {
            logger.LogWarning("[User: {UserId}] Failed to create subscription for device: {DeviceId}.", request.UserId, request.DeviceId);
            return Result<Operation>.Failure(subscriptionResponse.Message.Error);
        }

        var initializationResult = await factory
            .Initialize(request.DeviceId, subscriptionResponse.Message.SubscriptionId, request.Name, request.Type, request.Configuration, cancellationToken);

        if (initializationResult.IsFailure)
        {
            logger.LogWarning("[User: {UserId}] Failed to initialize operation for device: {DeviceId}.", request.UserId, request.DeviceId);
            return Result<Operation>.Failure(initializationResult.Errors);
        }

        //TODO: Redis lock for device

        var operationsCount = await queryRepository.CountOperationsByDevice(request.DeviceId, cancellationToken);

        if (options.Value.Limits.OperationsPerDevice <= operationsCount)
        {
            logger.LogWarning("[User: {UserId}] Operations limit reached for device: {DeviceId}.", request.UserId, request.DeviceId);
            return Result<Operation>.Failure(CreateOperationErrors.OperationsLimitReached);
        }

        var operation = initializationResult.Data.AsOperationEntity();

        var createResult = await commandRepository.CreateOperation(
            request.DeviceId,
            operation,
            cancellationToken);

        if (!createResult)
        {
            logger.LogError("[User: {UserId}] Failed to create operation for device: {DeviceId}.", request.UserId, request.DeviceId);
            return Result<Operation>.Failure(createResult.Errors);
        }

        logger.LogInformation("[User: {UserId}] Successfully created operation for device: {DeviceId}.", request.UserId, request.DeviceId);
        return Result<Operation>.Success(operation);
    }
}