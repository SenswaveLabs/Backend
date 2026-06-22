using System.Globalization;
using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Operations.Types.TextOperation;

[Trait("Collection", "UnitTests")]
public class ProcessPayloadTests : BaseTextTest
{
    [Theory]
    [InlineData("1.234")]
    [InlineData("-234.234")]
    [InlineData("sdfgsdfg")]
    [InlineData("#FFFFFF")]
    [InlineData("a-zA-Z0-9#+-_()[].,")]
    public void ProcessPayload(string payload)
    {
        // Arrange
        var operation = Operation;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(payload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(payload, result.Data.Value.GetValue<string>().ToString(CultureInfo.InvariantCulture));
    }

    [Fact]
    public void TooLongPayload()
    {
        //Arrange
        var operation = Operation;
        var payload = new string('A', 3000);

        //Act
        var operatinImplementation = factory.Create(operation);
        var value = operatinImplementation.Data.ProcessPayload(payload);

        //Assert
        Assert.True(value.IsFailure);
    }

    [Theory]
    [InlineData("\"-123\"")]
    [InlineData("\"0\"")]
    [InlineData("xyz$")]
    [InlineData("123,123!@#")]
    [InlineData("true*&^&")]
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
    [InlineData("{\"values\": \"#FFFFFF\", \"Value\": \"off\"}")]
    [InlineData("{\"values\": \"a-zA-Z0-9#+-_()[].,\", \"Value\": \"off\"}")]
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
    [InlineData("{\"values\": \"!123,23\", \"Value\": \"off\"}")]
    [InlineData("{\"values\": \")(&*(^%%$*%^^&*-123\"}")]
    [InlineData("{\"values\": true}")]
    [InlineData("{\"values\": 123}")]
    [InlineData("{\"values\": -123.09}")]
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

    [Fact]
    public void TooLongPayloadJson()
    {
        //Arrange
        var operation = JsonOperation;

        var request = new JsonObject
        {
            ["value"]= new string('a', 3000)
        };

        var payload = JsonSerializer.Serialize(request);

        //Act
        var operatinImplementation = factory.Create(operation);
        var value = operatinImplementation.Data.ProcessPayload(payload);

        //Assert
        Assert.True(value.IsFailure);
    }
}
