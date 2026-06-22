using Senswave.Devices.Domain.Widgets.Repositories;
using Senswave.Devices.Domain.Widgets.Services;

namespace Senswave.Devices.Application.Widgets.Features.DisplayWidgets;

internal sealed class DisplayWidgetsHandler(
    IWidgetAccessService accessService,
    IWidgetQueryRepository repository,
    ILogger<DisplayWidgetsHandler> logger) : IQueryHandler<DisplayWidgetsQuery, List<DisplayWidgetsGroupModel>>
{
    public async Task<Result<List<DisplayWidgetsGroupModel>>> Handle(DisplayWidgetsQuery request, CancellationToken cancellationToken)
    {
        var canDisplay = await accessService.CanDisplayDevice(request.UserId, request.DeviceId, cancellationToken);

        if (!canDisplay)
            return Result<List<DisplayWidgetsGroupModel>>.Failure(canDisplay.Errors);

        var operationsWithWidgets = await repository.GetOperationWithWidgets(request.DeviceId, cancellationToken);

        if (operationsWithWidgets.Count == 0)
        {
            logger.LogWarning("[UserId: {UserId}][DeviceId: {DeviceId}] No widgets found for device.", request.UserId, request.DeviceId);
            return Result<List<DisplayWidgetsGroupModel>>.Failure(DisplayWidgetsErrors.WidgetsNotFoundForDevice);
        }

        var displaysModel = operationsWithWidgets.Select(x => new DisplayWidgetsGroupModel
        {
            Operation = new()
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type
            },
            Widgets = x.Widgets
        }).ToList();

        logger.LogInformation("[UserId: {UserId}][DeviceId: {DeviceId}] Retrieved {Count} widget groups for device.",
            request.UserId, request.DeviceId, displaysModel.Count);
        return Result<List<DisplayWidgetsGroupModel>>.Success(displaysModel);
    }
}
