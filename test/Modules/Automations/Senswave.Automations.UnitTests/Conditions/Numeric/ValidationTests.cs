using System.Text.Json.Nodes;
using Xunit;

namespace Senswave.Automations.UnitTests.Conditions.Numeric;

[Trait("Collection", "UnitTests")]
public class ValidationTests : BaseTests
{
    [Fact]
    public async Task ValidateNumericCondition()
    {
        // Arrange
        var validJson = ToNumericAutomationImplementation(new JsonObject { ["minValue"] = 37, ["maxValue"] = 38 });
        var onlyMaxValueProvided = ToNumericAutomationImplementation(new JsonObject { ["maxValue"] = 37 });
        var onlyMinValueProvided = ToNumericAutomationImplementation(new JsonObject { ["minValue"] = 21 });

        var invalidJson = ToNumericAutomationImplementation(new JsonObject { ["mValue"] = 12, ["bigValue"] = 13 });
        var maxAndMinValueMixed = ToNumericAutomationImplementation(new JsonObject { ["minValue"] = 38, ["maxValue"] = 37 });

        // Act
        var valid = await validJson.Validate(new CancellationToken());
        var valid2 = await onlyMaxValueProvided.Validate(new CancellationToken());
        var valid3 = await onlyMinValueProvided.Validate(new CancellationToken());

        var inValid1 = await invalidJson.Validate(new CancellationToken());
        var inValid2 = await maxAndMinValueMixed.Validate(new CancellationToken());

        // Assert
        Assert.True(valid.IsSuccess);
        Assert.True(valid2.IsSuccess);
        Assert.True(valid3.IsSuccess);
        Assert.False(inValid1.IsSuccess);
        Assert.False(inValid2.IsSuccess);
    }
}