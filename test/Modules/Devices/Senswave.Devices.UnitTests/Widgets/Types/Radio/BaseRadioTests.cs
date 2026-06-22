using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Enums;
using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Widgets.Types.Radio;

public class BaseRadioTests : BaseWidgetTests
{
    protected Widget RadioWidget => new()
    {
        Id = Guid.NewGuid(),
        Name = "TestWidget",
        Type = WidgetType.Radio,
        Enabled = true,
        Configuration = new()
        {
            ["options"] = new JsonArray(
                new JsonObject { ["displayName"] = "Option 1", ["icon"] = "Icon1", ["optionName"] = "Option1" },
                new JsonObject { ["displayName"] = "Option-2", ["icon"] = "Icon1", ["optionName"] = "Option2" },
                new JsonObject { ["displayName"] = "Option 3", ["icon"] = "flash-outline", ["optionName"] = "Option3" }
            )
        },

        Operation = OptionsOperation,
        OperationId = OptionsOperation.Id,

        CreatedAtUtc = DateTime.UtcNow,
        UpdatedAtUtc = DateTime.UtcNow,
    };
}
