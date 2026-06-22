using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.Devices.Domain.Devices.Options;
using Senswave.Devices.Domain.Operations.Factory;
using Senswave.Devices.Domain.Operations.Models;
using Senswave.Devices.Domain.Operations.Repositories;
using Senswave.Devices.Domain.Operations.Services;
using Senswave.Integration.DataTransfer.MessageOperationProcessed;
using Senswave.Integration.DataTransfer.PublishMessageToDevice;
using System.Text.Json.Nodes;

namespace Senswave.Devices.Application.Operations.Services;

public class OperationActionService(
    OperationFactory factory,
    IPublishMessageBus bus,
    IOperationCommandRepository commandRepository,
    IOperationQueryRepository queryRepository,
    IOperationCleanupRepository cleanupRepository,
    IOptions<DevicesOptions> options,
    IRequestClient<PublishMessageToDeviceRequest> requestClient,
    ILogger<OperationActionService> logger) : IOperationActionService
{
    #region Error

    private readonly static Error AssemblingError = Error.Failure("FailedToAssembleMessage", "Failed to assemble the message payload.");

    private readonly static Error FailedToFindOperation = Error.Failure("FailedToFindOperation", "Failed to find the operation.");

    private readonly static Error FailedToUpdateOperationValue = Error.Failure("FailedToUpdateOperationValue", "Failed to update operation value.");

    private readonly static Error FailedToPublishMessage = Error.Failure("FailedToPublishMessage", "Failed to publish message to the broker.");

    private readonly static Error FailedToProcessMessage = Error.Failure("FailedToProcessMessage", "Failed to process the incoming message.");

    private readonly static Error FailedToCreateOperationForCreatingPayload = Error.ServerError("FailedToCreateOperationForCreatingPayload", "Failed to create operation for payload generation.");

    #endregion

    private readonly int _maxOperationValues = options.Value.Limits.OperationValuesPerOperation;

    public async Task<Result<List<Guid>>> IncomingOperationActionProcessing(Guid dataSourceSubscribtionReferenceId, string payload, CancellationToken cancellationToken)
    {
        var operations = await commandRepository.GetOperationsByReference(dataSourceSubscribtionReferenceId, cancellationToken);

        var processedOperations = new List<ProcessedOperationTriggerAutomationEvent>();

        foreach (var operation in operations)
        {
            var operationImplementation = factory.Create(operation);

            if (operationImplementation.IsFailure)
            {
                logger.LogCritical("[Operation: {operationId}] Failed to create operation implementation when processing payload.",
                    operation.Id);
                continue;
            }

            var result = operationImplementation.Data.ProcessPayload(payload);

            if (result.IsSuccess)
            {
                operation.Values.Add(result.Data);

                processedOperations.Add(new ProcessedOperationTriggerAutomationEvent
                {
                    HomeId = operation.Device.HomeReference.HomeId,
                    OperationId = operation.Id,
                });
            }
        }

        var updateSaved = await commandRepository.UpdateOperationsWithValue(operations, cancellationToken);

        if (!updateSaved)
        {
            logger.LogError("[DataSourceReferenceId {dataSourceReferenceId}] Failed to process message.",
                dataSourceSubscribtionReferenceId);
            return Result<List<Guid>>.Failure(FailedToProcessMessage);
        }

        foreach (var value in processedOperations)
        {
            await bus.Publish(value, cancellationToken);
        }

        var operationIds = operations
            .Select(x => x.Id)
            .ToList();

        await cleanupRepository
            .RemoveOperationValues(operationIds, _maxOperationValues, cancellationToken);

        logger.LogInformation("[DataSourceReferenceId {dataSourceReferenceId}] Successfully processed message for {Count} operations.",
            dataSourceSubscribtionReferenceId, operationIds.Count);

        return Result<List<Guid>>.Success(operationIds);
    }

    public async Task<Result<OperationAssembledModel>> OperationActionWithEvent(
        Guid operationId,
        JsonValue value,
        CancellationToken cancellationToken)
    {
        var sendingResult = await OperationAction(operationId, value, cancellationToken);

        if (!sendingResult)
        {
            logger.LogError("[Operation: {operationId}] Failed to send message to device", operationId);
            return sendingResult;
        }

        if (!sendingResult.Data.SendEvents)
        {
            logger.LogInformation("[Operation: {operationId}] Successfully sent updates to device without events.",
                operationId);

            return sendingResult;
        }

        try
        {
            var operation = await queryRepository.GetOperation(operationId, cancellationToken);

            if (operation is null)
            {
                logger.LogWarning("[Operation: {operationId}] Failed to send updates however operation was send to device.",
                    operationId);
                return Result<OperationAssembledModel>.Success(sendingResult.Data);
            }

            var processedEvent = new ProcessedOperationTriggerAutomationEvent
            {
                OperationId = operationId,
                HomeId = operation.Device?.HomeReference?.HomeId ?? Guid.Empty,
            };

            await bus.Publish(processedEvent, cancellationToken);

            logger.LogInformation("[Operation: {operationId}] Successfully sent updates to device.",
                operationId);

            return Result<OperationAssembledModel>.Success(sendingResult.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Operation: {operationId}] Failed to send updates however operation was send to device.",
                operationId);
            return Result<OperationAssembledModel>.Success(sendingResult.Data);
        }
    }

    public async Task<Result<OperationAssembledModel>> OperationAction(
        Guid operationId,
        JsonValue value,
        CancellationToken cancellationToken)
    {
        try
        {
            var operation = await commandRepository.GetOperation(operationId, cancellationToken);

            if (operation is null)
            {
                logger.LogWarning("[Operation: {operationId}] Failed to load operation.", operationId);
                return Result<OperationAssembledModel>.Failure(FailedToFindOperation);
            }

            var operationImplementationResult = factory.Create(operation);

            if (operationImplementationResult.IsFailure)
            {
                logger.LogCritical("[Operation: {operationId}] Failed to create operation implementation when creating payload.", operationId);
                return Result<OperationAssembledModel>.Failure(FailedToCreateOperationForCreatingPayload);
            }

            var operationImplementation = operationImplementationResult.Data;

            var assemblingResult = operationImplementation.CreatePayload(value);

            if (!assemblingResult)
            {
                logger.LogError("[Operation: {operationId}] Failed to assemble message.", operationId);
                return Result<OperationAssembledModel>.Failure(AssemblingError);
            }

            var publishMessageEvent = new PublishMessageToDeviceRequest()
            {
                DataSourceReferenceId = operation.DataReference!.DataSourceDataReferenceId,
                Payload = assemblingResult.Data.Payload.ToString()!,
            };

            var published = await requestClient.GetResponse<PublishMessageToDeviceResponse>(publishMessageEvent, cancellationToken);

            if (published.Message.IsFailure)
            {
                logger.LogError("[Operation: {operationId}] Failed to send message to device.", operationId);
                return Result<OperationAssembledModel>.Failure(FailedToPublishMessage, [published.Message.Error]);
            }

            var saveOperationValueOnUserAction = operationImplementation.Configuration.SaveOnUserAction;

            if (saveOperationValueOnUserAction)
            {
                logger.LogInformation("[Operation: {operationId}] Using data persistence on action.", operationId);

                await commandRepository.UpdateOperationWithValue(operation, assemblingResult.Data.Value, cancellationToken);
                await cleanupRepository.RemoveOperationValues(operation.Id, _maxOperationValues, cancellationToken);
                assemblingResult.Data.SendEvents = true;
            }

            logger.LogInformation("[Operation: {operationId}] Successfully assembled and sent message to device.", operationId);
            return Result<OperationAssembledModel>.Success(assemblingResult.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Operation: {operationId}] Failed to assemble and send message.", operationId);
            return Result<OperationAssembledModel>.Failure(FailedToPublishMessage);
        }
    }
}
