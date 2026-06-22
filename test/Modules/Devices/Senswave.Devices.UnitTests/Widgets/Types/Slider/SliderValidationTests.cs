using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Widgets.Types.Slider;

[Trait("Collection", "UnitTests")]
public class SliderValidationTests : BaseSliderTests
{
    [Fact]
    public async Task IntegerRangeRequired()
    {
        // Arrange
        var widget = IntegerWidget;
        widget.Configuration["step"] = JsonValue.Create(1);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public async Task ValidIntegerWidget(int value)
    {
        // Arrange
        var widget = IntegerRangedWidget;
        widget.Configuration["step"] = JsonValue.Create(value);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(1000)]
    [InlineData(-100)]
    [InlineData(0)]
    [InlineData(false)]
    [InlineData("asdas")]
    public async Task InvalidIntegerWidget(object value)
    {
        // Arrange
        var widget = IntegerRangedWidget;
        widget.Configuration["step"] = JsonValue.Create(value);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task NumberRangeRequired()
    {
        // Arrange
        var widget = NumberWidget;
        widget.Configuration["step"] = JsonValue.Create(1);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData(1.0)]
    [InlineData(100.0)]
    [InlineData(0.01)]
    public async Task ValidNumberWidget(double value)
    {
        // Arrange
        var widget = NumberRangedWidget;
        widget.Configuration["step"] = JsonValue.Create(value);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(1000)]
    [InlineData(-0.01)]
    [InlineData(0)]
    [InlineData(0.0001)]
    [InlineData(false)]
    [InlineData("asdas")]
    public async Task InvalidNumberWidget(object value)
    {
        // Arrange
        var widget = IntegerRangedWidget;
        widget.Configuration["step"] = JsonValue.Create(value);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.False(result.IsSuccess);
    }
}
