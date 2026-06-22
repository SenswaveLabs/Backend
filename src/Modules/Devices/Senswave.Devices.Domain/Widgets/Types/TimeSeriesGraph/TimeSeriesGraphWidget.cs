using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Devices.Domain.Operations.Types;
using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Models;
using Senswave.Devices.Domain.Widgets.Types.Base;
using System.Text.Json;

namespace Senswave.Devices.Domain.Widgets.Types.TimeSeriesGraph;

public sealed class TimeSeriesGraphWidget(
    Widget widget,
    IOperation operation,
    TimeSeriesGraphWidgetConfiguration configuration,
    ILogger<IWidget> logger) : BaseWidget(widget, operation)
{
    public override TimeSeriesGraphWidgetConfiguration Configuration => configuration;

    public override List<OperationType> AllowedOperationTypes => [
        OperationType.Integer,
        OperationType.Number
    ];

    public override Result<JsonValue> PreprocessAction(JsonValue incomingValue) =>
        Result<JsonValue>.Failure(BaseWidgetErrors.ActionIsNotSupported);

    public override async Task<DisplayWidgetModel> ToDisplay()
    {
        var displayModel = await base.ToDisplay();

        var config = new JsonObject()
        {
            ["displayUnit"] = Configuration.DisplayUnit,
            ["displayType"] = Configuration.DisplayTypeString,
            ["initialNumberOfData"] = Configuration.InitialNumberOfData
        };

        var getValueResult = await Operation.GetValuesToPastTime(Configuration.InitialNumberOfData);

        if (getValueResult.IsSuccess)
        {
            config["runtime"] = new JsonObject();

            var valuesArray = new JsonArray();

            foreach (var item in getValueResult.Data)
            {
                var dataToAdd = new JsonObject()
                {
                    ["value"] = item.Value.DeepClone(),
                    ["processedAtUtc"] = item.ProcessedAtUtc
                };

                valuesArray.Add(dataToAdd);
            }

            config["runtime"]!["values"] = valuesArray;
        }

        displayModel.Configuration = config;

        return displayModel;
    }

    public override Widget AsWidget()
    {
        var widget = base.AsWidget();
        widget.Configuration = new JsonObject
        {
            ["displayUnit"] = Configuration.DisplayUnit,
            ["displayType"] = Configuration.DisplayTypeString,
            ["initialNumberOfData"] = Configuration.InitialNumberOfData
        };

        return widget;
    }

    public override async Task<Result> Validate()
    {
        var result = await ValidateInternal<TimeSeriesGraphWidget, TimeSeriesGraphWidgetValidator>();

        if (result.IsFailure)
            logger.LogError("[Widget: {widgetId}] [WidgetType: {widgetType}] Base validation failed.", Id, Type);

        return result;
    }
}
