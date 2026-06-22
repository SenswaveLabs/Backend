using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Devices.Domain.Operations.Types;
using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Models;
using Senswave.Devices.Domain.Widgets.Types.Base;
using System.Text.Json;

namespace Senswave.Devices.Domain.Widgets.Types.Button;

public sealed class ButtonWidget(
    Widget widget,
    ButtonWidgetConfiguration configuration,
    IOperation operation,
    ILogger<IWidget> logger) : BaseWidget(widget, operation)
{
    public override ButtonWidgetConfiguration Configuration => configuration;

    public override List<OperationType> AllowedOperationTypes => [
        OperationType.Boolean,
        OperationType.Integer,
        OperationType.Number,
        OperationType.Text,
        OperationType.Options,
        OperationType.HexColor,
    ];

    public override async Task<Result> Validate()
    {
        var validationResult = await ValidateInternal<ButtonWidget, ButtonWidgetValidator>();

        if (validationResult.IsFailure)
        {
            logger.LogError("[Widget: {widgetId}] [WidgetType: {widgetType}] Base validation failed.", Id, Type);
            return validationResult;
        }

        var isValueCompliant = await Operation.IsValueCompliant(Configuration.Value);

        if (!isValueCompliant)
        {
            logger.LogError("[Widget: {widgetId}] [WidgetType: {widgetType}] Value '{value}' is not compliant with operation type '{operationType}'.",
                Id, Type, Configuration.Value, Operation.Type);

            return Result.Failure(BaseWidgetErrors.ValueIsNotCompliantWithOperation);
        }

        return Result.Success();
    }

    public override Result<JsonValue> PreprocessAction(JsonValue value)
    {
        if (!Enabled)
        {
            logger.LogWarning("[Widget {WidgetId}] Widget is not enabled.", Id);
            return Result<JsonValue>.Failure(BaseWidgetErrors.WidgetIsNotEnabled);
        }

        return Result<JsonValue>.Success(configuration.Value);
    }

    public override async Task<DisplayWidgetModel> ToDisplay()
    {
        var displayModel = await base.ToDisplay();

        var config = (JsonSerializer.SerializeToNode(Configuration) as JsonObject)!;

        displayModel.Configuration = config;

        return displayModel;
    }

    public override Widget AsWidget()
    {
        var widget = base.AsWidget();

        widget.Configuration = new JsonObject 
        { 
            ["value"] = Configuration.Value 
        };

        return widget;
    }
}
