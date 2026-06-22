using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Widgets.Entities;

namespace Senswave.Devices.Domain.Widgets.Repositories;

public interface IWidgetCommandRepository
{
    Task<Widget?> GetWidget(Guid widgetId, CancellationToken cancellationToken);

    Task<Result> CreateWidget(Widget widget, CancellationToken cancellationToken);

    Task<Result> UpdateWidget(Widget widget, CancellationToken cancellationToken);

    Task<Result> DeleteWidget(Guid widgetId, CancellationToken cancellationToken);

    Task<Operation?> GetOperationWithDevice(Guid operationId, CancellationToken cancellationToken);

    Task<bool> WidgetNameUsed(Guid operationId, string name, CancellationToken cancellationToken);
}