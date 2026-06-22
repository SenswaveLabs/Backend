using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Factory;
using Senswave.Devices.Domain.Widgets.Repositories;
using Senswave.Devices.Domain.Widgets.Services;

namespace Senswave.Devices.Application.Widgets.Features.CreateWidget;

public class CreateWidgetHandler(
    WidgetFactory factory,
    IWidgetAccessService widgetAccessService,
    IWidgetCommandRepository repository,
    ILogger<CreateWidgetHandler> logger) : ICommandHandler<CreateWidgetCommand, Widget>
{
    public async Task<Result<Widget>> Handle(CreateWidgetCommand request, CancellationToken cancellationToken)
    {
        var operation = await repository.GetOperationWithDevice(request.OperationId, cancellationToken);

        if (operation == null)
        {
            logger.LogError("[Operation:{operation}] Operation not found when creating widget.", request.OperationId);
            return Result<Widget>.Failure(CreateWidgetErrors.OperationNotFound);
        }

        var canManage = await widgetAccessService.CanManageDevice(request.UserId, operation.DeviceId, cancellationToken);

        if (!canManage)
        {
            return Result<Widget>.Failure(canManage.Errors);
        }

        var nameUsed = await repository.WidgetNameUsed(request.OperationId, request.Name, cancellationToken);

        if (nameUsed)
        {
            logger.LogError("[Device: {device}] Device name is already used.", operation.DeviceId);
            return Result<Widget>.Failure(CreateWidgetErrors.WidgetNameAlreadyUsed);
        }

        //TODO: Redis lock ?

        var widgetResult = await factory.TryInitialize(operation, request.Name, request.Type, request.Configuration);

        if (widgetResult.IsFailure)
        {
            logger.LogError("[Device: {device}] [Operation: {operation}] Failed to create widget for device.",
                operation.DeviceId,
                operation.Id);

            return Result<Widget>.Failure(widgetResult.Errors);
        }

        var result = await repository.CreateWidget(widgetResult.Data, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.LogError("[Device: {device}] [Operation: {operation}] Failed to save widget.",
                operation.DeviceId,
                operation.Id);

            return Result<Widget>.Failure(CreateWidgetErrors.FailedToSaveWidget, result.Errors);
        }

        logger.LogInformation("[Device: {device}] [Operation: {operation}] Widget created successfully.",
            operation.DeviceId,
            operation.Id);
        return Result<Widget>.Success(widgetResult.Data);
    }
}