using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Operations.Types.OptionOperation;

[Trait("Collection", "UnitTests")]
public class ValidateConfigurationTests : BaseOptionTest
{
    [Fact]
    public async Task ValidOperation()
    {
        // Arrange
        var operation = Operation;

        // Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ValidJsonOperation()
    {
        // Arrange
        var operation = JsonOperation;

        // Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task InvalidJsonOperation()
    {
        // Arrange
        var operation = JsonOperation;
        operation.Configuration["jsonNames"] = new JsonArray();

        // Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task TooFewOptions()
    {
        // Arrange
        var operation = Operation;
        operation.Configuration["options"] = new JsonArray(
            new JsonObject { ["name"] = "OnlyOne", ["value"] = "A" }
        );

        // Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task TooManyOptions()
    {
        // Arrange
        var operation = Operation;
        var options = new JsonArray();
        for (int i = 0; i < 11; i++)
        {
            options.Add(new JsonObject
            {
                ["name"] = $"Option{i}",
                ["value"] = $"{i}"
            });
        }
        operation.Configuration["options"] = options;

        // Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task DuplicateOptionNameNotAllowed()
    {
        // Arrange
        var operation = Operation;
        operation.Configuration["options"] = new JsonArray(
            new JsonObject { ["name"] = "Duplicate", ["value"] = "A" },
            new JsonObject { ["name"] = "Duplicate", ["value"] = "B" }
        );

        // Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task DuplicateValuesNameNotAllowed()
    {
        // Arrange
        var operation = Operation;
        operation.Configuration["options"] = new JsonArray(
            new JsonObject { ["name"] = "Option1", ["value"] = "Same" },
            new JsonObject { ["name"] = "Option2", ["value"] = "Same" }
        );

        // Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();

        // Assert
        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData("Invalid@Name")]
    [InlineData("name_with_")]
    [InlineData("!@#")]
    [InlineData("NameThatIsTooLong")]
    [InlineData("")]
    public async Task InvalidNames(string name)
    {
        // Arrange
        var operation = Operation;
        operation.Configuration["options"] = new JsonArray(
            new JsonObject { ["name"] = name, ["value"] = "Valid1" },
            new JsonObject { ["name"] = "ValidName2", ["value"] = "Valid2" }
        );

        // Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task TooLongTextValue()
    {
        // Arrange
        var longValue = new string('A', 65); // 65 ASCII characters = 65 bytes
        var operation = Operation;
        operation.Configuration["options"] = new JsonArray(
            new JsonObject { ["name"] = "ValidName", ["value"] = longValue },
            new JsonObject { ["name"] = "ValidName2", ["value"] = "Valid" }
        );

        // Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task MaxTextValueLenght()
    {
        // Arrange
        var value64 = new string('A', 64); // 64 ASCII characters = 64 bytes
        var operation = Operation;
        operation.Configuration["options"] = new JsonArray(
            new JsonObject { ["name"] = "ValidName", ["value"] = value64 },
            new JsonObject { ["name"] = "ValidName2", ["value"] = "Valid" }
        );

        // Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();

        // Assert
        Assert.True(result.IsSuccess);
    }
}
