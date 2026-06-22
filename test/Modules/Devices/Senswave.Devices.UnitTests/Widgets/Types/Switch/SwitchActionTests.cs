using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Widgets.Types.Switch;

[Trait("Collection", "UnitTests")]
public class SwitchActionTests : BaseSwitchTests
{
    [Fact]
    public async Task SwitchActionIsValid()
    {
        // Arrange
        var widget = BooleanWidget;
        var jsonValue = JsonValue.Create(true);
        widgetQueryRepository.Setup(x => x.GetWidgetWithOperation(widget.Id, default))
            .ReturnsAsync(widget);

        // Act
        var createdWidget = await widgetFactory.Create(widget.Id, default);
        var result = createdWidget.Data.PreprocessAction(jsonValue);

        // Assert
        Assert.True(createdWidget.IsSuccess);
        Assert.True(result.IsSuccess);
        Assert.Equal(jsonValue.ToString(), result.Data.ToString());
    }

    [Fact]
    public async Task EnabledFlagWorks()
    {
        var widget = BooleanWidget;
        widget.Enabled = false;
        var jsonValue = JsonValue.Create(!false);
        widgetQueryRepository.Setup(x => x.GetWidgetWithOperation(widget.Id, default))
            .ReturnsAsync(widget);

        // Act
        var createdWidget = await widgetFactory.Create(widget.Id, default);
        var result = createdWidget.Data.PreprocessAction(jsonValue);

        // Assert
        Assert.True(createdWidget.IsSuccess);
        Assert.False(result.IsSuccess);
    }
}
