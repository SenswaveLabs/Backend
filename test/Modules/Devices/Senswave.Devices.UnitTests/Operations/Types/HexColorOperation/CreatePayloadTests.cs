using Senswave.Devices.Domain.Operations.ValueObjects;
using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Operations.Types.HexColorOperation;

[Trait("Collection", "UnitTests")]
public class CreatePayloadTests : BaseHexColorTests
{
    [Theory]
    [InlineData("#FFF")]
    [InlineData("#000")]
    [InlineData("#FADFEE")]
    [InlineData("#fadfee")]
    public void CreatePayload(string input)
    {
        //Arrange
        var operation = Operation;

        var request = JsonValue.Create(input);

        var operationValue = new OperationValue
        {
            InternalValue = new JsonObject()
            {
                ["value"] = input
            }
        };

        //Act
        var operatinImplementation = factory.Create(operation);
        var value = operatinImplementation.Data.CreatePayload(request);

        //Assert
        Assert.True(value.IsSuccess);
        Assert.Equal(input.ToString(), value.Data.Payload);
        Assert.Equal(operationValue.Value.GetValue<string>(), value.Data.Value.Value.GetValue<string>());
    }

    [Theory]
    [InlineData("y")]
    [InlineData("#FFFF")]
    [InlineData("123y")]
    [InlineData(true)]
    [InlineData("abcdef")]
    [InlineData(1)]
    public void FailedToCreatePayload(object input)
    {
        //Arrange
        var operation = Operation;

        var request = JsonValue.Create(input)!;

        //Act
        var operatinImplementation = factory.Create(operation);
        var value = operatinImplementation.Data.CreatePayload(request);

        //Assert
        Assert.True(value.IsFailure);
    }

    [Theory]
    [InlineData("#FFF")]
    [InlineData("#000")]
    [InlineData("#FADFEE")]
    [InlineData("#fadfee")]
    public void CreatePayloadJson(string input)
    {
        //Arrange
        var operation = JsonOperation;
        var serializedOutput = JsonSerializer.Serialize(new JsonObject { ["color"] = input });

        var request = JsonValue.Create(input);

        var operationValue = new OperationValue
        {
            InternalValue = new JsonObject()
            {
                ["value"] = input
            }
        };

        //Act
        var operatinImplementation = factory.Create(operation);
        var value = operatinImplementation.Data.CreatePayload(request);

        //Assert
        Assert.True(value.IsSuccess);
        Assert.Equal(serializedOutput, value.Data.Payload);
        Assert.Equal(operationValue.Value.GetValue<string>(), value.Data.Value.Value.GetValue<string>());
    }

    [Theory]
    [InlineData("y")]
    [InlineData("#FFFF")]
    [InlineData("123y")]
    [InlineData(true)]
    [InlineData("abcdef")]
    [InlineData(1)]
    public void FailedToCreatePayloadJson(object input)
    {
        //Arrange
        var operation = JsonOperation;

        var request = JsonValue.Create(input)!;

        //Act
        var operatinImplementation = factory.Create(operation);
        var value = operatinImplementation.Data.CreatePayload(request);

        //Assert
        Assert.True(value.IsFailure);
    }
}
