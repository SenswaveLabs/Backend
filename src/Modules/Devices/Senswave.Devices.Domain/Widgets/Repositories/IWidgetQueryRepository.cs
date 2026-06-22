using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Widgets.Entities;

namespace Senswave.Devices.Domain.Widgets.Repositories;

public interface IWidgetQueryRepository
{
    Task<Widget?> GetWidgetWithOperation(Guid widgetId, CancellationToken cancellationToken);
    Task<List<Guid>> GetWidgetsByOperationId(Guid operationId, CancellationToken cancellationToken);
    Task<Guid> GetDeviceIdByOperations(List<Guid> operationIds, CancellationToken cancellationToken);
    Task<List<Guid>> GetWidgetsByOperationIds(List<Guid> operationIds, CancellationToken cancellationToken);
    Task<Guid> GetDeviceIdByWidgetId(Guid widgetId, CancellationToken cancellationToken);
    Task<List<Operation>> GetOperationWithWidgets(Guid deviceId, CancellationToken cancellationToken);
    Task<List<Widget>> GetWidgetsWithOperation(List<Guid> widgetIds, CancellationToken cancellationToken);
}