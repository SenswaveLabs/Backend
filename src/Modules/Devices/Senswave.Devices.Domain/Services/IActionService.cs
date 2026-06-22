namespace Senswave.Devices.Domain.Services;

public interface IActionService
{
    Task<Result> IncomingActionProcessing(List<Guid> operationIds, CancellationToken cancellationToken);

    Task<Result> TileAction(Guid deviceId, JsonValue value, CancellationToken cancellationToken);

    Task<Result> WidgetAction(Guid widgetId, JsonValue value, CancellationToken cancellationToken);

    Task<Result> ExternalAction(Guid operationId, JsonValue value, CancellationToken cancellationToken);
}
