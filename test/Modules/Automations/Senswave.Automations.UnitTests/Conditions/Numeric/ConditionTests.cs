using System.Text.Json.Nodes;
using Xunit;

namespace Senswave.Automations.UnitTests.Conditions.Numeric;

[Trait("Collection", "UnitTests")]
public class ConditionTests : BaseTests
{
    [Fact]
    public void CheckNumericCondition()
    {
        // Arrange
        var conditionWithBothBoundaries =
            ToNumericAutomationImplementation(new JsonObject { ["minValue"] = 37, ["maxValue"] = 41.5 });
        var conditionWithOnlyMaxValue =
            ToNumericAutomationImplementation(new JsonObject { ["maxValue"] = 41.5 });
        var conditionWithOnlyMinValue =
            ToNumericAutomationImplementation(new JsonObject { ["minValue"] = 37 });

        // Act & Assert
        Assert.True(conditionWithBothBoundaries.CheckCondition(JsonValue.Create(38.1)));
        Assert.True(conditionWithOnlyMaxValue.CheckCondition(JsonValue.Create(38.1)));
        Assert.True(conditionWithOnlyMinValue.CheckCondition(JsonValue.Create(38.1)));

        Assert.False(conditionWithBothBoundaries.CheckCondition(JsonValue.Create(36.9)));
        Assert.False(conditionWithOnlyMinValue.CheckCondition(JsonValue.Create(36.9)));

        Assert.False(conditionWithOnlyMaxValue.CheckCondition(JsonValue.Create(42.1)));
        Assert.False(conditionWithBothBoundaries.CheckCondition(JsonValue.Create(42.1)));

        Assert.False(conditionWithBothBoundaries.CheckCondition(JsonValue.Create("This is not a number")));
        Assert.False(conditionWithOnlyMaxValue.CheckCondition(JsonValue.Create("This is not a number")));
        Assert.False(conditionWithOnlyMinValue.CheckCondition(JsonValue.Create("This is not a number")));
    }
}