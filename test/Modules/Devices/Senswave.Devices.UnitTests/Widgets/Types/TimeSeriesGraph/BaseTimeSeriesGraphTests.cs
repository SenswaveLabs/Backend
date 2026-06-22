using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Enums;
using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Widgets.Types.TimeSeriesGraph;

public abstract class BaseTimeSeriesGraphTests : BaseWidgetTests
{
    protected Widget IntegerWidget => new()
    {
        Id = Guid.NewGuid(),
        Name = "TestWidget",
        Type = WidgetType.TimeSeriesGraph,
        Enabled = true,
        Configuration = ValidConfiguration(),
        Operation = IntegerRangedOperation,
        OperationId = IntegerRangedOperation.Id
    };

    protected Widget NumberWidget => new()
    {
        Id = Guid.NewGuid(),
        Name = "TestWidget",
        Type = WidgetType.TimeSeriesGraph,
        Enabled = true,
        Configuration = ValidConfiguration(),
        Operation = NumberRangedOperation,
        OperationId = NumberRangedOperation.Id
    };

    protected static JsonObject ValidConfiguration(
        string unit = "%",
        double timeRangeSeconds = 3600,
        int maxData = 100,
        string displayType = "lines") => new()
    {
        ["displayUnit"] = unit,
        ["defaultDisplayTimeRangeSeconds"] = timeRangeSeconds,
        ["initialNumberOfData"] = maxData,
        ["displayType"] = displayType
    };
}
