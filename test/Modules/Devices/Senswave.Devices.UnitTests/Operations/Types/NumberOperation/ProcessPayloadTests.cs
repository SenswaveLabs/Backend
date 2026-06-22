using System.Globalization;

namespace Senswave.Devices.UnitTests.Operations.Types.NumberOperation;

[Trait("Collection", "UnitTests")]
public class ProcessPayloadTests : BaseNumberTest
{
    [Theory]
    [InlineData("1.234")]
    [InlineData("-234.234")]
    public void ProcessPayload(string payload)
    {
        // Arrange
        var operation = Operation;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(payload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(payload, result.Data.Value.GetValue<double>().ToString(CultureInfo.InvariantCulture));
    }

    [Theory]
    [InlineData("1,234", 1.234)]
    [InlineData("-234,234", -234.234)]
    public void ProcessCommaPayload(string payload, double expectedValue)
    {
        // Arrange
        var operation = CommaOperation;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(payload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedValue, result.Data.Value.GetValue<double>());
        Assert.Equal(payload, result.Data.Value.GetValue<double>().ToString(new NumberFormatInfo { NumberDecimalSeparator = "," }));
    }

    [Theory]
    [InlineData("\"123\"")]
    [InlineData("\"-123\"")]
    [InlineData("\"0\"")]
    [InlineData("xyz")]
    [InlineData("123.123")]
    [InlineData("true")]
    public void FailedToProcessCommaPayload(string payload)
    {
        // Arrange
        var operation = CommaOperation;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(payload);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData("\"123\"")]
    [InlineData("\"-123\"")]
    [InlineData("\"0\"")]
    [InlineData("xyz")]
    [InlineData("123,123")]
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
    [InlineData("99.99")]
    [InlineData("-99.99")]
    public void ProcessPayloadRanged(string payload)
    {
        // Arrange
        var operation = RangedOperation;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(payload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(payload, result.Data.Value.GetValue<double>().ToString(CultureInfo.InvariantCulture));
    }

    [Theory]
    [InlineData("100.1")]
    [InlineData("100,1")]
    [InlineData("-100.11")]
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
    [InlineData("{\"values\": -2345.234, \"Value\": \"off\"}")]
    [InlineData("{\"values\": 123.23, \"Value\": \"off\"}")]
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
    [InlineData("{\"values\": \"123,23\", \"Value\": \"off\"}")]
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
    [InlineData("{\"detection\": -99.99, \"Value\": \"off\"}")]
    [InlineData("{\"detection\": 99.99, \"Value\": \"off\"}")]
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
    [InlineData("{\"detection\": -100.1, \"Value\": \"off\"}")]
    [InlineData("{\"detection\": 100.1}")]
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
