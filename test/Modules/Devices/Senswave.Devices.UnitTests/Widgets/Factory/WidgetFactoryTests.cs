using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Enums;

namespace Senswave.Devices.UnitTests.Widgets.Factory;

[Trait("Collection", "UnitTests")]
public class WidgetFactoryTests : BaseWidgetTests
{
    [Fact]
    public async Task OperationMustBeProvided()
    {
        // Arrange
        var widget = new Widget
        {
            Id = Guid.NewGuid(),

            Enabled = true,
            Name = "TestButton",
            Configuration = new()
            {
                ["value"] = true
            },

            Type = WidgetType.Button,
        };

        // Act
        var result = await widgetFactory.Create(widget.Id, default);

        // Assert
        Assert.True(result.IsFailure);

    }
}
