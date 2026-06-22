using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Enums;

namespace Senswave.Devices.UnitTests.Widgets.Types.Switch;

public abstract class BaseSwitchTests : BaseWidgetTests
{
    protected Widget BooleanWidget => new()
    {
        Id = Guid.NewGuid(),
        Name = "TestWidget",
        Type = WidgetType.Switch,
        Enabled = true,
        Configuration = [],
        Operation = BooleanOperation,
        OperationId = Guid.NewGuid(),
    };
}
