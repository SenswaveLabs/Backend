
using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Factory;
using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Enums;
using Senswave.Devices.Domain.Widgets.Repositories;
using Senswave.Devices.Domain.Widgets.Types;
using Senswave.Devices.Domain.Widgets.Types.Button;
using Senswave.Devices.Domain.Widgets.Types.Color;
using Senswave.Devices.Domain.Widgets.Types.Display;
using Senswave.Devices.Domain.Widgets.Types.Empty;
using Senswave.Devices.Domain.Widgets.Types.Invalid;
using Senswave.Devices.Domain.Widgets.Types.Radio;
using Senswave.Devices.Domain.Widgets.Types.Slider;
using Senswave.Devices.Domain.Widgets.Types.Switch;
using Senswave.Devices.Domain.Widgets.Types.TimeSeriesGraph;

namespace Senswave.Devices.Domain.Widgets.Factory;

public class WidgetFactory(
    OperationFactory operationFactory,
    IWidgetQueryRepository queryRepository,
    ILogger<WidgetFactory> logger,
    ILogger<IWidget> widgetLogger)
{
    #region Errors

    private static Error FailedToInitializeWidget => Error.Failure("FailedToInitializeWidget", "Failed to initialize widget.");

    private static Error FailedToCreateWidget => Error.Failure("FailedToCreateWidget", "Failed to create widget.");

    private static Error WidgetValidationFailed => Error.Failure("WidgetValidationFailed", "Failed to validate widget.");

    private static Error WidgetNotFound => Error.Failure("WidgetNotFound", "Widget not found.");

    #endregion

    public async Task<Result<Widget>> TryInitialize(Operation operation, string name, WidgetType type, JsonObject configuration)
    {
        var widget = new Widget
        {
            OperationId = operation.Id,
            Operation = operation,

            Name = name,
            Type = type,
            Enabled = true,
            Configuration = configuration,
        };

        var createWidgetResult = Create(widget);

        if (createWidgetResult.IsFailure)
        {
            logger.LogError("[Widget Type: {WidgetType}] Failed to initialize new widget.",
                widget.Type);

            return Result<Widget>.Failure(FailedToInitializeWidget);
        }

        var validationResult = await createWidgetResult.Data.Validate();

        if (validationResult.IsFailure)
        {
            logger.LogError("[Widget: {widget}] [Widget Type: {WidgetType}] Failed to validate new widget.",
                widget.Id,
                widget.Type);

            return Result<Widget>.Failure(WidgetValidationFailed);
        }

        var returnWidget = createWidgetResult.Data.AsWidget();

        return Result<Widget>.Success(returnWidget);
    }

    public async Task<Result<IWidget>> Create(Guid widgetId, CancellationToken cancellationToken)
    {
        var widget = await queryRepository.GetWidgetWithOperation(widgetId, cancellationToken);

        if (widget is null)
        {
            logger.LogError("[Widget: {widgetId}] Failed to find widget",
                widgetId);

            return Result<IWidget>.Failure(WidgetNotFound);
        }

        return Create(widget);
    }

    public async Task<Result<List<IWidget>>> Create(List<Guid> widgetIds, CancellationToken cancellationToken)
    {
        if (widgetIds.Count == 0)
            return Result<List<IWidget>>.Success([]);

        var widgetEntities = await queryRepository.GetWidgetsWithOperation(widgetIds, cancellationToken);

        var returnWidgets = new List<IWidget>();

        foreach (var id in widgetIds)
        {
            var widget = widgetEntities.FirstOrDefault(w => w.Id == id);

            if (widget is null)
            {
                logger.LogWarning("[Widget: {widgetId}] Failed to find widget", id);
                returnWidgets.Add(new EmptyWidget());
            }
            else
            {
                var parsedWidget = Create(widget);

                if (parsedWidget.IsFailure)
                {
                    logger.LogError("[Widget: {widget}] [Widget Type: {widgetType}] Failed to create widget.",
                        widget.Id,
                        widget.Type);

                    returnWidgets.Add(new InvalidWidget());
                }

                returnWidgets.Add(parsedWidget.Data);
            }
        }

        return Result<List<IWidget>>.Success(returnWidgets);
    }

    private Result<IWidget> Create(Widget widget)
    {
        try
        {
            if (widget.Operation is null)
            {
                logger.LogError("[Widget: {widget}] [Widget Type: {widgetType}] Operation is null when creating widget.",
                    widget.Id,
                    widget.Type);

                return Result<IWidget>.Failure(FailedToCreateWidget);
            }

            var operation = operationFactory.Create(widget.Operation);

            if (operation.IsFailure)
            {
                logger.LogError("[Widget: {widget}] [Operation: {operation}] Failed to create instance of operation.", widget.Id, widget.Operation.Id);

                return Result<IWidget>.Failure(operation.Errors);
            }

            if (widget.Type == WidgetType.Button)
                return Result<IWidget>.Success(widget.ToButtonWidget(operation.Data, widgetLogger));

            if (widget.Type == WidgetType.Display)
                return Result<IWidget>.Success(widget.ToDisplayWidget(operation.Data, widgetLogger));

            if (widget.Type == WidgetType.Slider)
                return Result<IWidget>.Success(widget.ToSliderWidget(operation.Data, widgetLogger));

            if (widget.Type == WidgetType.Switch)
                return Result<IWidget>.Success(widget.ToSwitchWidget(operation.Data, widgetLogger));

            if (widget.Type == WidgetType.Radio)
                return Result<IWidget>.Success(widget.ToRadioWidget(operation.Data, widgetLogger));

            if (widget.Type == WidgetType.Color)
                return Result<IWidget>.Success(widget.ToColorWidget(operation.Data, widgetLogger));

            if (widget.Type == WidgetType.TimeSeriesGraph)
                return Result<IWidget>.Success(widget.ToGraphWidget(operation.Data, widgetLogger));

            logger.LogError("[Widget: {widget}] [Widget Type: {widgetType}] Unknown widget type when creating.",
                widget.Id,
                widget.Type);

            return Result<IWidget>.Failure(Error.Failure("UnknownWidgetType", "Unknown widget type."));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Widget: {widget}] [Widget Type: {widgetType}] Failed to initialize widget.",
                widget.Id,
                widget.Type);

            return Result<IWidget>.Failure(FailedToCreateWidget);
        }
    }
}
