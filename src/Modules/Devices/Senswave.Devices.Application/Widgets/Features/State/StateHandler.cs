using Senswave.Devices.Domain.Widgets.Repositories;
using Senswave.Devices.Domain.Widgets.Services;

namespace Senswave.Devices.Application.Widgets.Features.State;

public class StateHandler(
    IWidgetAccessService accessService,
    IWidgetCommandRepository repository,
    ILogger<StateHandler> logger) : ICommandHandler<StateCommand>
{
    public async Task<Result> Handle(StateCommand request, CancellationToken cancellationToken)
    {
        var hasAccess = await accessService.CanManage(
            request.UserId,
            request.WidgetId,
            cancellationToken);

        if (!hasAccess)
        {
            logger.LogWarning("[User: {userId}] [Widget: {widgetId}] Widget has no privilege to act upon widget or widget not found.",
                request.UserId,
                request.WidgetId);

            return hasAccess;
        }

        var widget = await repository.GetWidget(request.WidgetId, cancellationToken);

        if (widget is null)
        {
            logger.LogError("[Widget: {widgetId}] Widget not found.",
                request.WidgetId);

            return Result.Failure(StateErrors.WidgetNotFound);
        }

        widget.Enabled = request.Enabled;

        var saveResult = await repository.UpdateWidget(widget, cancellationToken);

        if (saveResult.IsFailure)
        {
            logger.LogError("[Widget: {widgetId}] Failed to update widget",
                request.WidgetId);

            return Result.Failure(StateErrors.FailedToUpdateWidget, saveResult.Errors);
        }

        logger.LogInformation("[Widget: {widgetId}] Widget state updated successfully.",
            request.WidgetId);
        return Result.Success();
    }
}
