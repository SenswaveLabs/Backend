namespace Senswave.Devices.UnitTests.Widgets.Types.Switch;

[Trait("Collection", "UnitTests")]
public class SwitchValidationTests : BaseSwitchTests
{
    [Fact]
    public async Task NotValidIntegerWidget()
    {
        // Arrange
        var widget = BooleanWidget;
        widget.Operation = IntegerOperation;

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task NotValidNumberWidget()
    {
        // Arrange
        var widget = BooleanWidget;
        widget.Operation = NumberOperation;

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task NotValidTextWidget()
    {
        // Arrange
        var widget = BooleanWidget;
        widget.Operation = TextOperation;

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.False(result.IsSuccess);
    }


    [Fact]
    public async Task ValidBooleanWidget()
    {
        // Arrange
        var widget = BooleanWidget;

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsSuccess);
    }
}
