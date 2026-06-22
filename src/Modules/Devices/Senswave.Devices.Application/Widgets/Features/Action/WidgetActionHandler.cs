using Senswave.Devices.Domain.Services;
using Senswave.Devices.Domain.Widgets.Services;

namespace Senswave.Devices.Application.Widgets.Features.Action;

public sealed class WidgetActionHandler(
    IWidgetAccessService accessService,
    IActionService actionService,
    ILogger<WidgetActionHandler> logger) : ICommandHandler<WidgetActionCommand>
{
    public async Task<Result> Handle(WidgetActionCommand request, CancellationToken cancellationToken)
    {
        var canAct = await accessService.CanAct(request.UserId, request.WidgetId, cancellationToken);

        if (!canAct)
        {
            logger.LogWarning("[Widget: {widgetId}] User has no access to widget.", request.WidgetId);
            return Result.Failure(canAct.Errors);
        }

        var sendingResult = await actionService.WidgetAction(request.WidgetId, request.Value, cancellationToken);

        if (!sendingResult)
        {
            logger.LogError("[Widget: {widgetId}] Failed to send information to widget.", request.WidgetId);
            return Result.Failure(sendingResult.Errors);
        }

        logger.LogInformation("[Widget: {widgetId}] Information was sent to device.", request.WidgetId);
        return Result.Success();
    }
}
