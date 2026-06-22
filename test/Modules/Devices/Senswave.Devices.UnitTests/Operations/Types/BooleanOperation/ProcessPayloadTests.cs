namespace Senswave.Devices.UnitTests.Operations.Types.BooleanOperation;

[Trait("Collection", "UnitTests")]
public class ProcessPayloadTests : BaseBooleanTest
{
    [Theory]
    [InlineData("false", JsonValueKind.False)]
    [InlineData("true", JsonValueKind.True)]
    public void ProcessPayload(string payload, JsonValueKind expected)
    {
        // Arrange
        var operation = Operation;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(payload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Data.Value.GetValueKind());
    }

    [Theory]
    [InlineData("false", JsonValueKind.True)]
    [InlineData("true", JsonValueKind.False)]
    public void ProcessPayloadInversed(string payload, JsonValueKind expected)
    {
        // Arrange
        var operation = Operation;
        operation.Configuration["trueValue"] = false;
        operation.Configuration["falseValue"] = true;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(payload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Data.Value.GetValueKind());
    }

    [Theory]
    [InlineData("No", JsonValueKind.True)]
    [InlineData("Yes", JsonValueKind.False)]
    public void ProcessPayloadMappedText(string payload, JsonValueKind expected)
    {
        // Arrange
        var operation = Operation;
        operation.Configuration["trueValue"] = "No";
        operation.Configuration["falseValue"] = "Yes";

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(payload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Data.Value.GetValueKind());
    }

    [Theory]
    [InlineData("0", JsonValueKind.True)]
    [InlineData("9", JsonValueKind.False)]
    public void ProcessPayloadMappedNumber(string payload, JsonValueKind expected)
    {
        // Arrange
        var operation = Operation;
        operation.Configuration["trueValue"] = 0;
        operation.Configuration["falseValue"] = 9;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(payload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Data.Value.GetValueKind());
    }

    [Theory]
    [InlineData("0", JsonValueKind.True)]
    [InlineData("yes", JsonValueKind.False)]
    public void ProcessPayloadMappedMixed(string payload, JsonValueKind expected)
    {
        // Arrange
        var operation = Operation;
        operation.Configuration["trueValue"] = 0;
        operation.Configuration["falseValue"] = "yes";

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(payload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Data.Value.GetValueKind());
    }

    [Theory]
    [InlineData("\"false\"")]
    [InlineData("\"true\"")]
    [InlineData("\"xyz\"")]
    [InlineData("xyz")]
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
    [InlineData("{\"value1234\": false, \"Value\": \"off\"}", JsonValueKind.False)]
    [InlineData("{\"value1234\": true, \"Value\": \"off\"}", JsonValueKind.True)]
    public void ProcessPayloadJson(string jsonPayload, JsonValueKind expected)
    {
        // Arrange
        var operation = JsonOperation;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(jsonPayload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Data.Value.GetValueKind());
    }

    [Theory]
    [InlineData("{\"value1234\": false, \"Value\": \"off\"}", JsonValueKind.True)]
    [InlineData("{\"value1234\": true, \"Value\": \"off\"}", JsonValueKind.False)]
    public void ProcessPayloadJsonInverted(string jsonPayload, JsonValueKind expected)
    {
        // Arrange
        var operation = JsonOperation;
        operation.Configuration["trueValue"] = false;
        operation.Configuration["falseValue"] = true;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(jsonPayload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Data.Value.GetValueKind());
    }

    [Theory]
    [InlineData("{\"value1234\": false, \"Value\": \"off\"}", JsonValueKind.True)]
    [InlineData("{\"value1234\": \"nunu\", \"Value\": \"off\"}", JsonValueKind.False)]
    public void ProcessPayloadJsonMixed(string jsonPayload, JsonValueKind expected)
    {
        // Arrange
        var operation = JsonOperation;
        operation.Configuration["trueValue"] = false;
        operation.Configuration["falseValue"] = "nunu";

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(jsonPayload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Data.Value.GetValueKind());
    }

    [Theory]
    [InlineData("{\"value1234\": \"false\", \"Value\": \"off\"}")]
    [InlineData("{\"value1234\": \"true\", \"Value\": \"off\"}")]
    public void FailsToProcessPayloadJsonInverted(string jsonPayload)
    {
        // Arrange
        var operation = JsonOperation;
        operation.Configuration["trueValue"] = false;
        operation.Configuration["falseValue"] = true;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(jsonPayload);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData("{\"value1234\": \"Yes\", \"Value\": \"off\"}", JsonValueKind.False)]
    [InlineData("{\"value1234\": \"no\", \"Value\": \"off\"}", JsonValueKind.True)]
    public void ProcessPayloadJsonMappedText(string jsonPayload, JsonValueKind expected)
    {
        // Arrange
        var operation = JsonOperation;
        operation.Configuration["trueValue"] = "no";
        operation.Configuration["falseValue"] = "Yes";

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(jsonPayload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Data.Value.GetValueKind());
    }

    [Theory]
    [InlineData("{\"value1234\": 1, \"Value\": \"off\"}", JsonValueKind.False)]
    [InlineData("{\"value1234\": -1, \"Value\": \"off\"}", JsonValueKind.True)]
    public void ProcessPayloadJsonMappedNumber(string jsonPayload, JsonValueKind expected)
    {
        // Arrange
        var operation = JsonOperation;
        operation.Configuration["trueValue"] = -1;
        operation.Configuration["falseValue"] = 1;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(jsonPayload);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Data.Value.GetValueKind());
    }

    [Theory]
    [InlineData("{\"value1234\": \"1\", \"Value\": \"off\"}")]
    [InlineData("{\"value1234\": \"-1\", \"Value\": \"off\"}")]
    public void FailsToProcessPayloadJsonMappedNumber(string jsonPayload)
    {
        // Arrange
        var operation = JsonOperation;
        operation.Configuration["trueValue"] = -1;
        operation.Configuration["falseValue"] = 1;

        // Act
        var operatinImplementation = factory.Create(operation);
        var result = operatinImplementation.Data.ProcessPayload(jsonPayload);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData("{\"value1234\": \"false\", \"Value\": \"off\"}")]
    [InlineData("{\"value1234\": \"true\"}")]
    [InlineData("{\"value\": true}")]
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
}
