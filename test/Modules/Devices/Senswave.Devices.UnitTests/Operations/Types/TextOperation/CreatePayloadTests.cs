using Senswave.Devices.Domain.Operations.ValueObjects;
using System.Globalization;
using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Operations.Types.TextOperation;

[Trait("Collection", "UnitTests")]
public class CreatePayloadTests : BaseTextTest
{
    [Theory]
    [InlineData("dfg23452345")]
    [InlineData("sdfastrfgsdf")]
    [InlineData("cxvbdftghertyhftyujfgsdfghsrh")]
    [InlineData("")]
    [InlineData("#FFFFff")]
    public void CreatePayload(string input)
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

    [Fact]
    public void TooLongPayload()
    {
        //Arrange
        var operation = Operation;

        var request = JsonValue.Create(new string('A', 3000));

        //Act
        var operatinImplementation = factory.Create(operation);
        var value = operatinImplementation.Data.CreatePayload(request!);

        //Assert
        Assert.True(value.IsFailure);
    }

    [Theory]
    [InlineData(123)]
    [InlineData(123.345)]
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
    [InlineData("dfg23452345")]
    [InlineData("sdfastrfgsdf")]
    [InlineData("cxvbdftghertyhftyujfgsdfghsrh")]
    [InlineData("")]
    public void CreatePayloadJson(string input)
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
        Assert.Equal(operationValue.Value.GetValue<string>(), value.Data.Value.Value.GetValue<string>());
    }

    [Theory]
    [InlineData(123)]
    [InlineData(123.345)]
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

    [Fact]
    public void TooLongPayloadJson()
    {
        //Arrange
        var operation = JsonOperation;

        var request = JsonValue.Create(new string('a', 3000));

        //Act
        var operatinImplementation = factory.Create(operation);
        var value = operatinImplementation.Data.CreatePayload(request!);

        //Assert
        Assert.True(value.IsFailure);
    }
}
