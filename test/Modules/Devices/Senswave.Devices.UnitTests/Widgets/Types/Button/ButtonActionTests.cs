using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Widgets.Types.Button;

[Trait("Collection", "UnitTests")]
public class ButtonActionTests : BaseButtonTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ValuePreprocessed(bool value)
    {
        // Arrange
        var widget = BooleanWidget(value);
        var jsonValue = JsonValue.Create(!value);
        widgetQueryRepository.Setup(x => x.GetWidgetWithOperation(widget.Id, default))
            .ReturnsAsync(widget);

        // Act
        var createdWidget = await widgetFactory.Create(widget.Id, default);
        var result = createdWidget.Data.PreprocessAction(jsonValue);

        // Assert
        Assert.True(createdWidget.IsSuccess);
        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Data.GetValue<bool>());
    }

    [Fact]
    public async Task EnabledFlagWorks()
    {
        var widget = BooleanWidget(false);
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
