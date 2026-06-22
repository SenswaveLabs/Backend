using Senswave.Devices.Domain.Operations.ValueObjects;
using System.Globalization;
using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Operations.Types.NumberOperation;

[Trait("Collection", "UnitTests")]
public class CreatePayloadTests : BaseNumberTest
{
    [Theory]
    [InlineData(-101.23452345)]
    [InlineData(0)]
    [InlineData(1010.12039)]
    public void CreatePayload(double input)
    {
        //Arrange
        var operation = Operation;

        var request = JsonValue.Create(input);

        //Act
        var operatinImplementation = factory.Create(operation);
        var value = operatinImplementation.Data.CreatePayload(request!);

        //Assert
        Assert.True(value.IsSuccess);
        Assert.Equal(input.ToString(CultureInfo.InvariantCulture), value.Data.Payload);
    }

    [Theory]
    [InlineData("y")]
    [InlineData("12334557tre6")]
    [InlineData("123y")]
    [InlineData("123,345")]
    [InlineData(true)]
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
    [InlineData(-101.234)]
    [InlineData(1010.12039)]
    public void CreatePayloadComma(double input)
    {
        //Arrange
        var operation = CommaOperation;

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

        //Assert
        Assert.True(value.IsSuccess);
        Assert.Contains(",", value.Data.Payload);
        Assert.Equal(input.ToString(new NumberFormatInfo { NumberDecimalSeparator = "," }), value.Data.Payload);
    }

    [Theory]
    [InlineData(-99.99)]
    [InlineData(100)]
    [InlineData(23.23)]
    public void CreatePayloadRanged(double input)
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
        var value = operatinImplementation.Data.CreatePayload(request!);

        //Assert
        Assert.True(value.IsSuccess);
        Assert.Equal(input.ToString(CultureInfo.InvariantCulture), value.Data.Payload);
        Assert.Equal(operationValue.Value.GetValue<double>(), value.Data.Value.Value.GetValue<double>());
    }

    [Theory]
    [InlineData(-101.23)]
    [InlineData(101.4)]
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
    [InlineData(102.234)]
    [InlineData(2340.2)]
    [InlineData(22)]
    public void CreatePayloadJson(double input)
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
        Assert.Equal(operationValue.Value.GetValue<double>(), value.Data.Value.Value.GetValue<double>());
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
    [InlineData(99.99)]
    [InlineData(-99.99)]
    public void CreatePayloadRangedJson(double input)
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
        Assert.Equal(operationValue.Value.GetValue<double>(), value.Data.Value.Value.GetValue<double>());
    }

    [Theory]
    [InlineData(-100.1)]
    [InlineData(100.1)]
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
