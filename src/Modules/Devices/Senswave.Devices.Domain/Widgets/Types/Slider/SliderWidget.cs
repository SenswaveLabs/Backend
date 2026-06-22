using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Devices.Domain.Operations.Types;
using Senswave.Devices.Domain.Operations.Types.Characteristics.Range;
using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Models;
using Senswave.Devices.Domain.Widgets.Types.Base;

namespace Senswave.Devices.Domain.Widgets.Types.Slider;

public class SliderWidget(Widget widget,
    SliderWidgetConfiguration configuration,
    IOperation operation,
    ILogger<IWidget> logger) : BaseWidget(widget, operation)
{
    public override List<OperationType> AllowedOperationTypes => [
        OperationType.Integer,
        OperationType.Number
    ];

    public override SliderWidgetConfiguration Configuration => configuration;

    public override Result<JsonValue> PreprocessAction(JsonValue incomingValue)
    {
        if (!Enabled)
        {
            logger.LogWarning("[Widget {WidgetId}] Widget is not enabled.", Id);
            return Result<JsonValue>.Failure(BaseWidgetErrors.WidgetIsNotEnabled);
        }

        return Result<JsonValue>.Success(incomingValue);
    }

    public override async Task<DisplayWidgetModel> ToDisplay()
    {
        var displayModel = await base.ToDisplay();

        var config = new JsonObject()
        {
            ["step"] = JsonValue.Create(Configuration.Step)
        };

        if (Operation is IRangeCharacteristic rangeCharacteristic)
        {
            var rangeResult = await rangeCharacteristic.GetDisplayRange();

            if (rangeResult.IsFailure)
            {
                // todo log failure
            }

            config["range"] = rangeResult.Data;
        }

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
            ["step"] = JsonValue.Create(Configuration.Step) 
        };

        return widget;
    }

    public override async Task<Result> Validate()
    {
        var validationResult = await ValidateInternal<SliderWidget, SliderWidgetValidator>();

        if (validationResult.IsFailure)
        {
            logger.LogError("[Widget: {widgetId}] [WidgetType: {widgetType}] Base validation failed.", Id, Type);
            return validationResult;
        }

        if (Operation is IRangeCharacteristic rangeCharacteristic)
        {
            var characteristicResult = await rangeCharacteristic.ValidateStep(Configuration.Step!);

            if (characteristicResult.IsFailure)
                logger.LogError("[Widget: {widgetId}] [WidgetType: {widgetType}] Failed to validate characteristic.", Id, Type);

            return characteristicResult;
        }

        return Result.Failure(Error.Validation("RangeCharacteristicTypeNotSupported", "Range Characteristic type is not supported"));
    }
}
