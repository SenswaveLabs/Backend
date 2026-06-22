using Senswave.Devices.Domain.Widgets.Factory;
using Senswave.Devices.Domain.Widgets.Models;
using Senswave.Devices.Domain.Widgets.Services;
using System.Text.Json.Nodes;

namespace Senswave.Devices.Application.Widgets.Services;

internal class WidgetService(
    WidgetFactory widgetFactory,
    ILogger<WidgetService> logger) : IWidgetService
{
    public async Task<Result<WidgetMessageModel>> PreprocessValueForWidgetMessage(Guid widgetId, JsonValue value, CancellationToken cancellationToken)
    {
        var widget = await widgetFactory.Create(widgetId, cancellationToken);

        if (widget.IsFailure)
        {
            logger.LogCritical("[Widget: {widgetId}] Failed to create widget.", widgetId);
            return Result<WidgetMessageModel>.Failure(widget.Errors);
        }

        var preprocessingResult = widget.Data
            .PreprocessAction(value);

        if (preprocessingResult.IsFailure)
        {
            logger.LogError("[Widget: {widgetId}] Failed to preprocess value for widget.", widgetId);
            return Result<WidgetMessageModel>.Failure(preprocessingResult.Errors);
        }

        var message = new WidgetMessageModel
        {
            OperationId = widget.Data.Operation.Id,
            Value = preprocessingResult.Data
        };

        return Result<WidgetMessageModel>.Success(message);
    }

    public async Task<Result<DisplayWidgetModel>> Interpret(Guid widgetId, CancellationToken cancellationToken)
    {
        var widget = await widgetFactory.Create(widgetId, cancellationToken);

        if (widget.IsFailure)
        {
            logger.LogError("[Widget: {widgetId}] Failed to create widget for interpretation.", widgetId);
            return Result<DisplayWidgetModel>.Failure(widget.Errors);
        }

        var displayModel = await widget.Data.ToDisplay();

        return Result<DisplayWidgetModel>.Success(displayModel);
    }
}
