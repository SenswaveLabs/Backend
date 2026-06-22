namespace Senswave.Devices.UnitTests.Widgets.Types.TimeSeriesGraph;

[Trait("Collection", "UnitTests")]
public class TimeSeriesGraphValidationTests : BaseTimeSeriesGraphTests
{
    [Fact]
    public async Task ValidIntegerWidget()
    {
        var widget = IntegerWidget;

        var result = await BaseValidationTest(widget);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ValidNumberWidget()
    {
        var widget = NumberWidget;

        var result = await BaseValidationTest(widget);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("aA1±+-")]
    [InlineData("°µΩ‰%?℃℉")]
    [InlineData("")]
    [InlineData("ms")]
    [InlineData("%")]
    public async Task ValidUnits(string unit)
    {
        var widget = IntegerWidget;
        widget.Configuration["displayUnit"] = unit;

        var result = await BaseValidationTest(widget);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("true!/4")]
    [InlineData("abcdersazxa")]
    [InlineData("/\\|\'\"")]
    [InlineData("()[]{}")]
    [InlineData("=;:?")]
    [InlineData(".,@#$^*")]
    public async Task InvalidUnits(string unit)
    {
        var widget = IntegerWidget;
        widget.Configuration["displayUnit"] = unit;

        var result = await BaseValidationTest(widget);

        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1000)]
    public async Task ValidInitialNumberOfData(int maxData)
    {
        var widget = IntegerWidget;
        widget.Configuration["initialNumberOfData"] = maxData;

        var result = await BaseValidationTest(widget);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1001)]
    public async Task InvalidInitialNumberOfData(int maxData)
    {
        var widget = IntegerWidget;
        widget.Configuration["initialNumberOfData"] = maxData;

        var result = await BaseValidationTest(widget);

        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData("lines")]
    [InlineData("points")]
    [InlineData("bars")]
    public async Task ValidDisplayTypes(string displayType)
    {
        var widget = IntegerWidget;
        widget.Configuration["displayType"] = displayType;

        var result = await BaseValidationTest(widget);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("unknown")]
    [InlineData("")]
    [InlineData("empty")]
    public async Task InvalidDisplayTypes(string displayType)
    {
        var widget = IntegerWidget;
        widget.Configuration["displayType"] = displayType;

        var result = await BaseValidationTest(widget);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task BooleanOperationNotSupported()
    {
        var widget = IntegerWidget;
        widget.Operation = BooleanOperation;

        var result = await BaseValidationTest(widget);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task TextOperationNotSupported()
    {
        var widget = IntegerWidget;
        widget.Operation = TextOperation;

        var result = await BaseValidationTest(widget);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task OptionsOperationNotSupported()
    {
        var widget = IntegerWidget;
        widget.Operation = OptionsOperation;

        var result = await BaseValidationTest(widget);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task HexColorOperationNotSupported()
    {
        var widget = IntegerWidget;
        widget.Operation = HexColorOperation;

        var result = await BaseValidationTest(widget);

        Assert.True(result.IsFailure);
    }
}
