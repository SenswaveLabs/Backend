using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Operations.Types.OptionOperation;

[Trait("Collection", "UnitTests")]
public class CreatePayloadTests : BaseOptionTest
{
    [Theory]
    [InlineData("Option1")]
    [InlineData("Option2")]
    [InlineData("Option3")]
    [InlineData("Option4")]
    public void ValidOptions(string input)
    {
        // Arrange
        var operation = Operation;
        var request = JsonValue.Create(input);

        var expectedValue = operation.Configuration["options"]!
            .AsArray()
            .OfType<JsonObject>()
            .FirstOrDefault(o => o["name"]!.GetValue<string>() == input)!;

        // Act
        var implementation = factory.Create(operation).Data;
        var result = implementation.CreatePayload(request!);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedValue["value"]!.ToString(), result.Data.Payload);
        Assert.Equal(expectedValue["name"]!.ToString(), result.Data.Value.Value.ToString());
    }

    [Theory]
    [InlineData("InvalidOption")]
    [InlineData("OptionX")]
    [InlineData("Random")]
    [InlineData(true)]
    public void InvalidOptions(object input)
    {
        // Arrange
        var operation = Operation;
        var request = JsonValue.Create(input);

        // Act
        var implementation = factory.Create(operation).Data;
        var result = implementation.CreatePayload(request!);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData("Option1")]
    [InlineData("Option2")]
    public void ValidJsonOptions(string input)
    {
        // Arrange
        var operation = JsonOperation;
        var request = JsonValue.Create(input);

        var expectedValue = operation.Configuration["options"]!
            .AsArray()
            .OfType<JsonObject>()
            .FirstOrDefault(o => o["name"]!.GetValue<string>() == input);

        var expectedPayload = JsonSerializer.Serialize(new JsonObject
        {
            ["values"] = expectedValue!["value"]!.DeepClone()
        });

        // Act
        var implementation = factory.Create(operation).Data;
        var result = implementation.CreatePayload(request!);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedPayload, result.Data.Payload);
        Assert.Equal(expectedValue["name"]!.ToString(), result.Data.Value.Value.ToString());
    }

    [Theory]
    [InlineData("InvalidOption")]
    [InlineData("AnotherInvalid")]
    public void InvalidJsonOptions(string input)
    {
        // Arrange
        var operation = JsonOperation;
        var request = JsonValue.Create(input);

        // Act
        var implementation = factory.Create(operation).Data;
        var result = implementation.CreatePayload(request!);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData(12.31)]
    [InlineData(1)]
    [InlineData(true)]
    [InlineData("NotMatching")]
    [InlineData(9999)]
    [InlineData(false)]
    public void CreatePayload_InvalidNonStringValues(object input)
    {
        // Arrange
        var operation = Operation;
        var request = JsonValue.Create(input);

        // Act
        var implementation = factory.Create(operation).Data;
        var result = implementation.CreatePayload(request!);

        // Assert
        Assert.True(result.IsFailure);
    }
}
