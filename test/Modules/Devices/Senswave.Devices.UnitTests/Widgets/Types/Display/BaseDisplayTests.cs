using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Enums;

namespace Senswave.Devices.UnitTests.Widgets.Types.Display;

public abstract class BaseDisplayTests : BaseWidgetTests
{
    protected Widget NumberRangedWidget => new()
    {
        Id = Guid.NewGuid(),
        Name = "TestWidget",
        Type = WidgetType.Display,
        Enabled = true,
        Configuration = new()
        {
            ["unit"] = "%"
        },
        Operation = NumberRangedOperation,
        OperationId = Guid.NewGuid(),
    };

    protected Widget IntegerRangedWidget => new()
    {
        Id = Guid.NewGuid(),
        Name = "TestWidget",
        Type = WidgetType.Display,
        Enabled = true,
        Configuration = new()
        {
            ["unit"] = "°C"
        },
        Operation = IntegerRangedOperation,
        OperationId = IntegerRangedOperation.Id
    };

    protected Widget TextWidget => new()
    {
        Id = Guid.NewGuid(),
        Name = "TestWidget",
        Type = WidgetType.Display,
        Enabled = true,
        Configuration = new()
        {
            ["unit"] = "ms"
        },
        Operation = TextOperation,
        OperationId = TextOperation.Id
    };
}
