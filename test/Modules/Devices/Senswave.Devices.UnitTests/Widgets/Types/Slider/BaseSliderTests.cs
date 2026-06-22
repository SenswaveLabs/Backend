using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Enums;

namespace Senswave.Devices.UnitTests.Widgets.Types.Slider;

public abstract class BaseSliderTests : BaseWidgetTests
{
    protected Widget NumberRangedWidget => new()
    {
        Id = Guid.NewGuid(),
        Name = "TestWidget",
        Type = WidgetType.Slider,
        Enabled = true,
        Configuration = new()
        {
            ["step"] = 0.01
        },
        Operation = NumberRangedOperation,
        OperationId = Guid.NewGuid(),
    };

    protected Widget IntegerRangedWidget => new()
    {
        Id = Guid.NewGuid(),
        Name = "TestWidget",
        Type = WidgetType.Slider,
        Enabled = true,
        Configuration = new()
        {
            ["step"] = 1
        },
        Operation = IntegerRangedOperation,
        OperationId = IntegerRangedOperation.Id
    };

    protected Widget IntegerWidget => new()
    {
        Id = Guid.NewGuid(),
        Name = "TestWidget",
        Type = WidgetType.Slider,
        Enabled = true,
        Configuration = new()
        {
            ["step"] = 1
        },
        Operation = IntegerOperation,
        OperationId = IntegerOperation.Id
    };

    protected Widget NumberWidget => new()
    {
        Id = Guid.NewGuid(),
        Name = "TestWidget",
        Type = WidgetType.Slider,
        Enabled = true,
        Configuration = new()
        {
            ["step"] = 0.01
        },
        Operation = NumberOperation,
        OperationId = NumberOperation.Id,
    };
}
