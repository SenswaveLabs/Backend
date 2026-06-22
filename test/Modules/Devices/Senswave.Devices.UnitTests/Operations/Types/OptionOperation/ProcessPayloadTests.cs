namespace Senswave.Devices.UnitTests.Operations.Types.OptionOperation;

[Trait("Collection", "UnitTests")]
public class ProcessPayloadTests : BaseOptionTest
{
    [Theory]
    [InlineData("Value", "Option1")]
    [InlineData("12.31", "Option2")]
    [InlineData("12,31", "Option2")]
    [InlineData("1", "Option3")]
    [InlineData("true", "Option4")]
    public void ValidPayload(string payload, string expectedOptionName)
    {
        // Arrange
        var operation = Operation;

        // Act
        var operatinImplementation = factory.Create(operation).Data;
        var result = operatinImplementation.ProcessPayload(payload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedOptionName, result.Data.Value.ToString());
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("12345")]
    [InlineData("false")]
    [InlineData("12.315")]
    [InlineData("12,315")]
    public void InvalidPayload(string payload)
    {
        // Arrange
        var operation = Operation;

        // Act
        var operatinImplementation = factory.Create(operation).Data;
        var result = operatinImplementation.ProcessPayload(payload);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData("{\"values\": \"Value\"}", "Option1")]
    [InlineData("{\"values\": 12.31}", "Option2")]
    [InlineData("{\"values\": 1}", "Option3")]
    [InlineData("{\"values\": true}", "Option4")]
    public void ValidJsonPayload(string jsonPayload, string expectedOptionName)
    {
        // Arrange
        var operation = JsonOperation;

        // Act
        var operatinImplementation = factory.Create(operation).Data;
        var result = operatinImplementation.ProcessPayload(jsonPayload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedOptionName, result.Data.Value.ToString());
    }

    [Theory]
    [InlineData("{\"values\": \"Option1\"}")]
    [InlineData("{\"values\": 123.45}")]
    [InlineData("{\"values\": false}")]
    [InlineData("{\"values\": null}")]
    public void InvalidJsonPayload(string jsonPayload)
    {
        // Arrange
        var operation = JsonOperation;

        // Act
        var operatinImplementation = factory.Create(operation).Data;
        var result = operatinImplementation.ProcessPayload(jsonPayload);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData("Option1")]
    [InlineData("Option2")]
    public void InvalidPrimitiveNameInsteadOfValue(string payload)
    {
        // Arrange
        var operation = Operation;

        // Act
        var operatinImplementation = factory.Create(operation).Data;
        var result = operatinImplementation.ProcessPayload(payload);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData("{\"wrongKey\": \"Value\"}")]
    [InlineData("{\"other\": 1}")]
    public void InvalidJsonKey(string jsonPayload)
    {
        // Arrange
        var operation = JsonOperation;

        // Act
        var operatinImplementation = factory.Create(operation).Data;
        var result = operatinImplementation.ProcessPayload(jsonPayload);

        // Assert
        Assert.False(result.IsSuccess);
    }
}
