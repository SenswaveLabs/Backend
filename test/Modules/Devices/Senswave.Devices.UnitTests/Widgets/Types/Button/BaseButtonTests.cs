using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Enums;
using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Widgets.Types.Button;

public abstract class BaseButtonTests : BaseWidgetTests
{
    protected Widget BooleanWidget(object value) => new()
    {
        Id = Guid.NewGuid(),
        Name = "TestWidget",
        Type = WidgetType.Button,
        Enabled = true,
        Configuration = new()
        {
            ["value"] = JsonValue.Create(value)
        },
        Operation = BooleanOperation,
        OperationId = Guid.NewGuid(),
    };

    protected Widget NumberRanngedWidget(object value) => new()
    {
        Id = Guid.NewGuid(),
        Name = "TestWidget",
        Type = WidgetType.Button,
        Enabled = true,
        Configuration = new()
        {
            ["value"] = JsonValue.Create(value)
        },
        Operation = NumberRangedOperation,
        OperationId = Guid.NewGuid(),
    };

    protected Widget IntegerRangedWidget(object value) => new()
    {
        Id = Guid.NewGuid(),
        Name = "TestWidget",
        Type = WidgetType.Button,
        Enabled = true,
        Configuration = new()
        {
            ["value"] = JsonValue.Create(value)
        },
        Operation = IntegerRangedOperation,
        OperationId = IntegerRangedOperation.Id
    };

    protected Widget TextWidget(object value) => new()
    {
        Id = Guid.NewGuid(),
        Name = "TestWidget",
        Type = WidgetType.Button,
        Enabled = true,
        Configuration = new()
        {
            ["value"] = JsonValue.Create(value)
        },
        Operation = TextOperation,
        OperationId = Guid.NewGuid(),
    };

    protected Widget OptionWidget(string optionName = "Option1") => new()
    {
        Id = Guid.NewGuid(),
        Name = "TestWidget",
        Type = WidgetType.Button,
        Enabled = true,
        Configuration = new()
        {
            ["value"] = JsonValue.Create(optionName)
        },
        Operation = OptionsOperation,
        OperationId = Guid.NewGuid(),
    };

    protected Widget HexColorWidget(object color) => new()
    {
        Id = Guid.NewGuid(),
        Name = "TestWidget",
        Type = WidgetType.Button,
        Enabled = true,
        Configuration = new()
        {
            ["value"] = JsonValue.Create(color)
        },
        Operation = HexColorOperation,
        OperationId = Guid.NewGuid(),
    };
}
