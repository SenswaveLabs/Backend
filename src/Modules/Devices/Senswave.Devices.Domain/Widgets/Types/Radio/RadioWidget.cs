using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Devices.Domain.Operations.Types;
using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Models;
using Senswave.Devices.Domain.Widgets.Types.Base;
using System.Text.Json;

namespace Senswave.Devices.Domain.Widgets.Types.Radio;

public class RadioWidget(
    Widget widget,
    RadioWidgetConfiguration configuration,
    IOperation opeartion,
    ILogger<IWidget> logger) : BaseWidget(widget, opeartion)
{
    public override List<OperationType> AllowedOperationTypes => [
        OperationType.Options
    ];

    public override RadioWidgetConfiguration Configuration => configuration;

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

        var config = new JsonObject
        {
            ["options"] = JsonSerializer.SerializeToNode(Configuration.Options)
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
        widget.Configuration = (JsonSerializer.SerializeToNode(Configuration) as JsonObject)!;

        return widget;
    }

    public override async Task<Result> Validate()
    {
        var validationResult = await ValidateInternal<RadioWidget, RadioWidgetValidator>();

        if (validationResult.IsFailure)
            return validationResult;

        foreach (var option in Configuration.Options)
        {
            var optionValue = JsonValue.Create(option.OptionName);
            var isCompliant = await Operation.IsValueCompliant(optionValue);

            if (isCompliant.IsFailure)
            {
                logger.LogWarning("[Widget: {widgetId}] [Operation: {operationId}] Option value '{optionValue}' is not compliant with operation type '{operationType}'.",
                    Id, Operation.Id, optionValue, Operation.Type);

                return Result.Failure(BaseWidgetErrors.ValueIsNotCompliantWithOperation);
            }
        }

        return Result.Success();
    }
}
