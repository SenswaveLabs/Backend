using Senswave.Devices.Domain.Operations.ValueObjects;
using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Operations.Types.BooleanOperation;

[Trait("Collection", "UnitTests")]
public class CreatePayloadTests : BaseBooleanTest
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CreatePayload(bool input)
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
        Assert.Equal(input.ToString().ToLower(), value.Data.Payload);
        Assert.Equal(operationValue.Value.GetValueKind(), value.Data.Value.Value.GetValueKind());
    }

    [Theory]
    [InlineData(false, "No")]
    [InlineData(true, "Yes")]
    public void CreatePayloadMappedText(bool input, string expected)
    {
        //Arrange
        var operation = Operation;
        operation.Configuration["trueValue"] = "Yes";
        operation.Configuration["falseValue"] = "No";

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
        Assert.Equal(expected, value.Data.Payload);
        Assert.Equal(operationValue.Value.GetValueKind(), value.Data.Value.Value.GetValueKind());
    }

    [Theory]
    [InlineData(false, -1)]
    [InlineData(true, 1)]
    public void CreatePayloadMappedNumber(bool input, int expected)
    {
        //Arrange
        var operation = Operation;
        operation.Configuration["trueValue"] = 1;
        operation.Configuration["falseValue"] = -1;

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
        Assert.Equal(expected.ToString(), value.Data.Payload);
        Assert.Equal(operationValue.Value.GetValueKind(), value.Data.Value.Value.GetValueKind());
    }

    [Theory]
    [InlineData(false, "true")]
    [InlineData(true, "false")]
    public void CreatePayloadInverted(bool input, string expected)
    {
        //Arrange
        var operation = Operation;
        operation.Configuration["trueValue"] = false;
        operation.Configuration["falseValue"] = true;

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
        Assert.Equal(expected, value.Data.Payload);
        Assert.Equal(operationValue.Value.GetValueKind(), value.Data.Value.Value.GetValueKind());
    }

    [Theory]
    [InlineData(false, "yes")]
    [InlineData(true, "0")]
    public void CreatePayloadMixedTypes(bool input, string expected)
    {
        //Arrange
        var operation = Operation;
        operation.Configuration["trueValue"] = 0;
        operation.Configuration["falseValue"] = "yes";

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
        Assert.Equal(expected, value.Data.Payload);
        Assert.Equal(operationValue.Value.GetValueKind(), value.Data.Value.Value.GetValueKind());
    }

    [Theory]
    [InlineData("y")]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData("1")]
    public void FailedToCreatePayload(object input)
    {
        //Arrange
        var operation = Operation;
        var request = JsonValue.Create(input);

        //Act
        var operatinImplementation = factory.Create(operation);
        var value = operatinImplementation.Data.CreatePayload(request!);

        //Assert
        Assert.True(value.IsFailure);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CreatePayloadJson(bool input)
    {
        //Arrange
        var serializedOutput = JsonSerializer.Serialize(new JsonObject { ["value1234"] = input });
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
        var value = operatinImplementation.Data.CreatePayload(request);

        // Assert
        Assert.True(value.IsSuccess);
        Assert.Equal(serializedOutput, value.Data.Payload);
        Assert.Equal(operationValue.Value.GetValueKind(), value.Data.Value.Value.GetValueKind());
    }

    [Theory]
    [InlineData(false, "No")]
    [InlineData(true, "Yes")]
    public void CreatePayloadJsonMappedText(bool input, string expected)
    {
        //Arrange
        var serializedOutput = JsonSerializer.Serialize(new JsonObject { ["value1234"] = expected });
        var operation = JsonOperation;
        operation.Configuration["trueValue"] = "Yes";
        operation.Configuration["falseValue"] = "No";

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

        // Assert
        Assert.True(value.IsSuccess);
        Assert.Equal(serializedOutput, value.Data.Payload);
        Assert.Equal(operationValue.Value.GetValueKind(), value.Data.Value.Value.GetValueKind());
    }

    [Theory]
    [InlineData(false, -3)]
    [InlineData(true, 1233)]
    public void CreatePayloadJsonMappedNumber(bool input, int expected)
    {
        //Arrange
        var serializedOutput = JsonSerializer.Serialize(new JsonObject { ["value1234"] = expected });
        var operation = JsonOperation;
        operation.Configuration["trueValue"] = 1233;
        operation.Configuration["falseValue"] = -3;

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

        // Assert
        Assert.True(value.IsSuccess);
        Assert.Equal(serializedOutput, value.Data.Payload);
        Assert.Equal(operationValue.Value.GetValueKind(), value.Data.Value.Value.GetValueKind());
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void CreatePayloadJsonInverted(bool input, bool expected)
    {
        //Arrange
        var serializedOutput = JsonSerializer.Serialize(new JsonObject { ["value1234"] = expected });
        var operation = JsonOperation;
        operation.Configuration["trueValue"] = false;
        operation.Configuration["falseValue"] = true;

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

        // Assert
        Assert.True(value.IsSuccess);
        Assert.Equal(serializedOutput, value.Data.Payload);
        Assert.Equal(operationValue.Value.GetValueKind(), value.Data.Value.Value.GetValueKind());
    }

    [Fact]
    public void CreatePayloadJsonMixedTypes_TrueInput()
    {
        //Arrange
        var serializedOutput = JsonSerializer.Serialize(new JsonObject { ["value1234"] = false });
        var operation = JsonOperation;
        operation.Configuration["trueValue"] = false;
        operation.Configuration["falseValue"] = "nunu";

        var request = JsonValue.Create(true);

        //Act
        var operatinImplementation = factory.Create(operation);
        var value = operatinImplementation.Data.CreatePayload(request);

        // Assert
        Assert.True(value.IsSuccess);
        Assert.Equal(serializedOutput, value.Data.Payload);
        Assert.Equal(JsonValueKind.True, value.Data.Value.Value.GetValueKind());
    }

    [Fact]
    public void CreatePayloadJsonMixedTypes_FalseInput()
    {
        //Arrange
        var serializedOutput = JsonSerializer.Serialize(new JsonObject { ["value1234"] = "nunu" });
        var operation = JsonOperation;
        operation.Configuration["trueValue"] = false;
        operation.Configuration["falseValue"] = "nunu";

        var request = JsonValue.Create(false);

        //Act
        var operatinImplementation = factory.Create(operation);
        var value = operatinImplementation.Data.CreatePayload(request);

        // Assert
        Assert.True(value.IsSuccess);
        Assert.Equal(serializedOutput, value.Data.Payload);
        Assert.Equal(JsonValueKind.False, value.Data.Value.Value.GetValueKind());
    }

    [Theory]
    [InlineData("y")]
    public void FailedToCreatePayloadJson(string input)
    {
        //Arrange
        var operation = JsonOperation;

        var request = JsonValue.Create(input);

        //Act
        var operatinImplementation = factory.Create(operation);
        var value = operatinImplementation.Data.CreatePayload(request);

        //Assert
        Assert.True(value.IsFailure);
    }
}
