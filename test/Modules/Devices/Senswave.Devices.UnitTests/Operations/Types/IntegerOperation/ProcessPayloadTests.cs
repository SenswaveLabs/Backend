namespace Senswave.Devices.UnitTests.Operations.Types.IntegerOperation;

[Trait("Collection", "UnitTests")]
public class ProcessPayloadTests : BaseIntegerTest
{
    [Theory]
    [InlineData("1")]
    [InlineData("-234")]
    public void ProcessPayload(string payload)
    {
        // Arrange
        var operation = Operation;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(payload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(payload, result.Data.Value.GetValue<int>().ToString());
    }

    [Theory]
    [InlineData("\"123\"")]
    [InlineData("\"-123\"")]
    [InlineData("\"0\"")]
    [InlineData("xyz")]
    [InlineData("true")]
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
    [InlineData("100")]
    [InlineData("-100")]
    public void ProcessPayloadRanged(string payload)
    {
        // Arrange
        var operation = RangedOperation;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(payload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(payload, result.Data.Value.GetValue<int>().ToString());
    }

    [Theory]
    [InlineData("101")]
    [InlineData("-101")]
    public void FailedToProcessPayloadRanged(string payload)
    {
        // Arrange
        var operation = RangedOperation;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(payload);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData("{\"values\": -2345, \"Value\": \"off\"}")]
    [InlineData("{\"values\": 123, \"Value\": \"off\"}")]
    public void ProcessPayloadJson(string jsonPayload)
    {
        // Arrange
        var operation = JsonOperation;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(jsonPayload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(JsonValueKind.Number, result.Data.Value.GetValueKind());
    }


    [Theory]
    [InlineData("{\"values\": \"123\", \"Value\": \"off\"}")]
    [InlineData("{\"values\": \"-123\"}")]
    [InlineData("{\"values\": true}")]
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

    [Theory]
    [InlineData("{\"detection\": -100, \"Value\": \"off\"}")]
    [InlineData("{\"detection\": 100, \"Value\": \"off\"}")]
    public void ProcessPayloadRangedJson(string jsonPayload)
    {
        // Arrange
        var operation = JsonRangedOperation;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(jsonPayload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(JsonValueKind.Number, result.Data.Value.GetValueKind());
    }


    [Theory]
    [InlineData("{\"detection\": -101, \"Value\": \"off\"}")]
    [InlineData("{\"detection\": 101}")]
    public void FailedToProcessPayloadRangedJson(string jsonPayload)
    {
        // Arrange
        var operation = JsonRangedOperation;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(jsonPayload);

        // Assert
        Assert.False(result.IsSuccess);
    }
}
