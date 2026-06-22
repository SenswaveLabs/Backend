using Senswave.Devices.Domain.Widgets.Models;

namespace Senswave.Devices.Domain.Widgets.Services;

public interface IWidgetService
{
    Task<Result<WidgetMessageModel>> PreprocessValueForWidgetMessage(Guid widgetId, JsonValue value, CancellationToken cancellationToken);

    Task<Result<DisplayWidgetModel>> Interpret(Guid widgetId, CancellationToken cancellationToken);
}
