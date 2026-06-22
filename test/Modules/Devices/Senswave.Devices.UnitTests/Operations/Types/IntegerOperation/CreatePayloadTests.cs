using Senswave.Devices.Domain.Operations.ValueObjects;
using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Operations.Types.IntegerOperation;

[Trait("Collection", "UnitTests")]
public class CreatePayloadTests : BaseIntegerTest
{
    [Theory]
    [InlineData(-101)]
    [InlineData(0)]
    [InlineData(1010)]
    public void CreatePayload(int input)
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
        Assert.Equal(operationValue.Value.GetValue<int>(), value.Data.Value.Value.GetValue<int>());
    }

    [Theory]
    [InlineData("y")]
    [InlineData("123")]
    [InlineData("123y")]
    [InlineData(true)]
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
    [InlineData(-100)]
    [InlineData(100)]
    public void CreatePayloadRanged(int input)
    {
        //Arrange
        var operation = RangedOperation;

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
        Assert.Equal(operationValue.Value.GetValue<int>(), value.Data.Value.Value.GetValue<int>());
    }

    [Theory]
    [InlineData(-101)]
    [InlineData(101)]
    public void FailedToCreatePayloadRanged(object input)
    {
        //Arrange
        var operation = RangedOperation;

        var request = JsonValue.Create(input);

        //Act
        var operatinImplementation = factory.Create(operation);
        var value = operatinImplementation.Data.CreatePayload(request!);

        //Assert
        Assert.True(value.IsFailure);
    }

    [Theory]
    [InlineData(102)]
    [InlineData(2340)]
    public void CreatePayloadJson(int input)
    {
        //Arrange
        var serializedOutput = JsonSerializer.Serialize(new JsonObject { ["values"] = input });
        var operation = JsonOperation;

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
        var value = operatinImplementation.Data.CreatePayload(request!);

        // Assert
        Assert.True(value.IsSuccess);
        Assert.Equal(serializedOutput, value.Data.Payload);
        Assert.Equal(operationValue.Value.GetValue<int>(), value.Data.Value.Value.GetValue<int>());
    }

    [Theory]
    [InlineData("y")]
    [InlineData(true)]
    public void FailedToCreatePayloadJson(object input)
    {
        //Arrange
        var operation = JsonOperation;

        var request = JsonValue.Create(input);

        //Act
        var operatinImplementation = factory.Create(operation);
        var value = operatinImplementation.Data.CreatePayload(request!);

        //Assert
        Assert.True(value.IsFailure);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(-100)]
    public void CreatePayloadRangedJson(int input)
    {
        //Arrange
        var serializedOutput = JsonSerializer.Serialize(new JsonObject { ["detection"] = input });
        var operation = JsonRangedOperation;

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
        var value = operatinImplementation.Data.CreatePayload(request!);

        // Assert
        Assert.True(value.IsSuccess);
        Assert.Equal(serializedOutput, value.Data.Payload);
        Assert.Equal(operationValue.Value.GetValue<int>(), value.Data.Value.Value.GetValue<int>());
    }

    [Theory]
    [InlineData(-101)]
    [InlineData(101)]
    [InlineData(true)]
    public void FailedToCreatePayloadRangedJson(object input)
    {
        //Arrange
        var operation = JsonRangedOperation;

        var request = JsonValue.Create(input);

        //Act
        var operatinImplementation = factory.Create(operation);
        var value = operatinImplementation.Data.CreatePayload(request!);

        //Assert
        Assert.True(value.IsFailure);
    }
}
