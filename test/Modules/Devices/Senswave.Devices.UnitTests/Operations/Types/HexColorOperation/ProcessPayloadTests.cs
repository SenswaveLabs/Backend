namespace Senswave.Devices.UnitTests.Operations.Types.HexColorOperation;

[Trait("Collection", "UnitTests")]
public class ProcessPayloadTests : BaseHexColorTests
{
    [Theory]
    [InlineData("#FFF")]
    [InlineData("#ABCDEF")]
    public void ProcessPayload(string payload)
    {
        // Arrange
        var operation = Operation;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(payload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(payload, result.Data.Value.GetValue<string>().ToString());
    }

    [Theory]
    [InlineData("\"#FFF\"")]
    [InlineData("1")]
    [InlineData("false")]
    [InlineData("#FGF")]
    [InlineData("FFABCD")]
    public void FailedToProcessPayload(string payload)
    {
        // Arrange
        var operation = Operation;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(payload);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData("{\"color\": \"#ffFFff\", \"Value\": \"off\"}")]
    [InlineData("{\"color\": \"#00aa0b\", \"Value\": \"off\"}")]
    public void ProcessPayloadJson(string jsonPayload)
    {
        // Arrange
        var operation = JsonOperation;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(jsonPayload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(JsonValueKind.String, result.Data.Value.GetValueKind());
    }


    [Theory]
    [InlineData("{\"color\": \"#1234\", \"Value\": \"off\"}")]
    [InlineData("{\"color\": \"ffffff\"}")]
    [InlineData("{\"color\": true}")]
    [InlineData("{\"values\": \"#00aa0b\", \"Color\": \"off\"}")]
    public void FailedToProcessPayloadJson(string jsonPayload)
    {
        // Arrange
        var operation = JsonOperation;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(jsonPayload);

        // Assert
        Assert.False(result.IsSuccess);
    }
}
