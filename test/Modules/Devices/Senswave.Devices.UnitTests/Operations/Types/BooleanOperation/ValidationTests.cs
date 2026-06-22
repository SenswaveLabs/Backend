using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Operations.Types.BooleanOperation;

[Trait("Collection", "UnitTests")]
public class ValidationTests : BaseBooleanTest
{
    [Fact]
    public async Task Validate()
    {
        // Arrange
        var operation = Operation;

        // Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();
        var parsedOperation = implementation.AsOperationEntity();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(parsedOperation.Configuration["isJson"]!.GetValue<bool>());
    }

    [Fact]
    public async Task ValidateJson()
    {
        // Arrange
        var operation = JsonOperation;

        // Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();
        var parsedOperation = implementation.AsOperationEntity();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(parsedOperation.Configuration["isJson"]!.GetValue<bool>());
    }

    [Theory]
    [InlineData("tolongfieldnametopersistindatbaseforoperationtolongfieldnametopersistindatbaseforoperationtolongfieldnametopersistindatbaseforoperationtolongfieldnametopersistindatbaseforoperation")]
    [InlineData("")]
    public async Task InvalidFieldNameInJsonOperation(string name)
    {
        // Arrange
        var tooLongConfigurationConfiguration = new JsonObject
        {
            ["jsonNames"] = new JsonArray("values", name),
            ["isJson"] = true
        };

        var operation = JsonOperation;
        operation.Configuration = tooLongConfigurationConfiguration;

        // Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();

        // Assert
        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task JsonConfigurationRequiresFieldNames()
    {
        // Arange
        var configuration = new JsonObject
        {
            ["isJson"] = true,
        };

        var operation = JsonOperation;
        operation.Configuration = configuration;

        //Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();

        //Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task OverflowInFieldsIsIgnored()
    {
        // Arange
        var operation = JsonOperation;
        operation.Configuration["overflow"] = new JsonArray("values", "true");

        //Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();
        var parsedOperation = implementation.AsOperationEntity();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.False(parsedOperation.Configuration.ContainsKey("overflow"));
    }

    [Fact]
    public async Task TooMuchJsonNesting()
    {
        // Arange
        var operation = JsonOperation;
        operation.Configuration["jsonNames"] = new JsonArray("values", "true", "true", "true", "true", "true");

        //Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();

        //Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task MaximalJsonNesting()
    {
        // Arange
        var operation = JsonOperation;
        operation.Configuration["jsonNames"] = new JsonArray("values", "true", "true", "true", "true");

        //Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();
        var parsedOperation = implementation.AsOperationEntity();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(5, parsedOperation.Configuration["jsonNames"]!.AsArray().Count);
    }

    [Fact]
    public async Task MissingFalseValue()
    {
        // Arange
        var operation = JsonOperation;
        operation.Configuration["trueValue"] = -1;

        //Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();

        //Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task MissingTrueValue()
    {
        // Arange
        var operation = JsonOperation;
        operation.Configuration["falseValue"] = 1;

        //Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();

        //Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ValuesCannotBeEqual()
    {
        // Arange
        var operation = JsonOperation;
        operation.Configuration["falseValue"] = 1;
        operation.Configuration["trueValue"] = 1;

        //Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();

        //Assert
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData("tolongfieldnametopersistindatbaseforoperationtolongfieldnametopersistindatbaseforoperationtolongfieldnametopersistindatbaseforoperationtolongfieldnametopersistindatbaseforoperation")]
    [InlineData("")]
    public async Task InvalidArgumentLength(string data)
    {
        // Arange
        var operation = JsonOperation;
        operation.Configuration["trueValue"] = 1;
        operation.Configuration["trueValue"] = data;

        //Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();

        //Assert
        Assert.False(result.IsSuccess);
    }
}
