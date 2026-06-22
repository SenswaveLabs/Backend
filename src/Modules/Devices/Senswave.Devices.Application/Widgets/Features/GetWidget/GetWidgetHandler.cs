using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Repositories;
using Senswave.Devices.Domain.Widgets.Services;

namespace Senswave.Devices.Application.Widgets.Features.GetWidget;

public class GetWidgetHandler(
    IWidgetAccessService accessService,
    IWidgetQueryRepository repository,
    ILogger<GetWidgetHandler> logger) : IQueryHandler<GetWidgetQuery, Widget>
{
    public async Task<Result<Widget>> Handle(GetWidgetQuery request, CancellationToken cancellationToken)
    {
        var canDisplay = await accessService.CanDisplay(request.UserId, request.WidgetId, cancellationToken);

        if (!canDisplay.IsSuccess)
            return Result<Widget>.Failure(canDisplay.Errors);

        var widget = await repository.GetWidgetWithOperation(request.WidgetId, cancellationToken);

        if (widget is null)
        {
            logger.LogWarning("[User: {userId}] [Widget: {widgetId}] Widget not found or user does not have access to it.",
                request.UserId,
                request.WidgetId);
            return Result<Widget>.Failure(GetWidgetErrors.WidgetNotFound);
        }

        logger.LogInformation("[User: {userId}] [Widget: {widgetId}] Widget retrieved successfully.",
            request.UserId,
            request.WidgetId);
        return Result<Widget>.Success(widget);
    }
}