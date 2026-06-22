using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Devices.Domain.Operations.Types;
using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Models;
using Senswave.Devices.Domain.Widgets.Types.Base;

namespace Senswave.Devices.Domain.Widgets.Types.Switch;

public class SwitchWidget(
    Widget widget,
    SwitchWidgetConfiguration configuration,
    IOperation operation,
    ILogger<IWidget> logger) : BaseWidget(widget, operation)
{
    public override List<OperationType> AllowedOperationTypes => [
        OperationType.Boolean,
    ];

    public override SwitchWidgetConfiguration Configuration => configuration;

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

        var config = new JsonObject();

        var getValueResult = await Operation.GetCurrentValue();

        if (getValueResult.IsSuccess)
            config["runtime"] = getValueResult.Data.InternalValue;

        displayModel.Configuration = config;

        return displayModel;
    }

    public override async Task<Result> Validate()
    {
        var result = await ValidateInternal<SwitchWidget, SwitchWidgetValidator>();

        if (result.IsFailure)
            logger.LogError("[Widget: {widgetId}] [WidgetType: {widgetType}] Base validation failed.", Id, Type);

        return result;
    }
}
