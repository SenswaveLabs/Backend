using Senswave.Devices.Domain.Widgets.Repositories;
using Senswave.Devices.Domain.Widgets.Services;

namespace Senswave.Devices.Application.Widgets.Features.DeleteWidget;

public class DeleteWidgetHandler(
    IWidgetAccessService accessService,
    IWidgetCommandRepository commandRepository,
    ILogger<DeleteWidgetHandler> logger) : ICommandHandler<DeleteWidgetCommand>
{
    public async Task<Result> Handle(DeleteWidgetCommand request, CancellationToken cancellationToken)
    {
        var isOwner = await accessService.CanManage(request.OwnerId, request.WidgetId, cancellationToken);

        if (!isOwner)
        {
            logger.LogWarning("[User: {userId}] [Widget: {widgetId}] User does not have access to delete the widget.",
                request.OwnerId,
                request.WidgetId);
            return Result.Failure(DeleteWidgetErrors.NoAccess);
        }

        var result = await commandRepository.DeleteWidget(request.WidgetId, cancellationToken);

        if (!result)
        {
            logger.LogError("[Widget: {widgetId}] Failed to delete widget.",
                request.WidgetId);
            return Result.Failure(DeleteWidgetErrors.FailedToRemoveWidget, result.Errors);
        }

        logger.LogInformation("[Widget: {widgetId}] Widget deleted successfully.",
            request.WidgetId);
        return Result.Success();
    }
}