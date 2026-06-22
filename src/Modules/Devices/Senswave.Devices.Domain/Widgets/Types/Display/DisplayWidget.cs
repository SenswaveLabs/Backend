using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Devices.Domain.Operations.Types;
using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Models;
using Senswave.Devices.Domain.Widgets.Types.Base;

namespace Senswave.Devices.Domain.Widgets.Types.Display;

public sealed class DisplayWidget(
    Widget widget,
    IOperation operation,
    DisplayWidgetConfiguration configuration,
    ILogger<IWidget> logger) : BaseWidget(widget, operation)
{
    public override DisplayWidgetConfiguration Configuration => configuration;

    public override List<OperationType> AllowedOperationTypes => [
        OperationType.Integer,
        OperationType.Number,
        OperationType.Text
    ];

    public override Result<JsonValue> PreprocessAction(JsonValue incomingValue) =>
        Result<JsonValue>.Failure(BaseWidgetErrors.ActionIsNotSupported);

    public override async Task<DisplayWidgetModel> ToDisplay()
    {
        var displayModel = await base.ToDisplay();

        var config = new JsonObject()
        {
            ["unit"] = Configuration.Unit
        };

        var getValueResult = await Operation.GetCurrentValue();

        if (getValueResult.IsSuccess)
            config["runtime"] = getValueResult.Data.InternalValue;

        displayModel.Configuration = config;

        return displayModel;
    }

    public override Widget AsWidget()
    {
        var widget = base.AsWidget();
        widget.Configuration = new JsonObject 
        { 
            ["unit"] = Configuration.Unit 
        };

        return widget;
    }

    public override async Task<Result> Validate()
    {
        var result = await ValidateInternal<DisplayWidget, DisplayWidgetValidator>();

        if (result.IsFailure)
            logger.LogError("[Widget: {widgetId}] [WidgetType: {widgetType}] Base validation failed.", Id, Type);

        return result;
    }
}
