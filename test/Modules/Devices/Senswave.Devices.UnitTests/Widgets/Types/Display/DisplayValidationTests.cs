namespace Senswave.Devices.UnitTests.Widgets.Types.Display;

[Trait("Collection", "UnitTests")]
public class DisplayValidationTests : BaseDisplayTests
{
    [Fact]
    public async Task ValidIntegerWidget()
    {
        // Arrange
        var widget = IntegerRangedWidget;

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ValidNumberWidget()
    {
        // Arrange
        var widget = NumberRangedWidget;

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ValidTextWidget()
    {
        // Arrange
        var widget = TextWidget;

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("aA1±+-")]
    [InlineData("°µΩ‰%?℃℉")]
    [InlineData("")]
    [InlineData("ms")]
    public async Task ValidUnits(string unit)
    {
        // Arrange
        var widget = IntegerRangedWidget;
        widget.Configuration["unit"] = unit;

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("true!/4")]
    [InlineData("abcdersazxa")]
    [InlineData("/\\|\'\"")]
    [InlineData("()[]{}")]
    [InlineData("=;:?")]
    [InlineData(".,@#$^*")]
    public async Task UnitValidationWorks(string unit)
    {
        // Arrange
        var widget = IntegerRangedWidget;
        widget.Configuration["unit"] = unit;

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task BooleanOperationNotValid()
    {
        // Arrange
        var widget = IntegerRangedWidget;
        widget.Operation = BooleanOperation;

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsFailure);
    }
}
