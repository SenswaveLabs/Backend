using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Enums;

namespace Senswave.Devices.UnitTests.Widgets.Types.Color;

public class BaseColorTests : BaseWidgetTests
{
    protected Widget ColorWidget => new()
    {
        Id = Guid.NewGuid(),
        Name = "TestWidget",
        Type = WidgetType.Color,
        Enabled = true,
        Configuration = [],
        Operation = HexColorOperation,
        OperationId = Guid.NewGuid(),
    };
}
