using System.Text.Json.Nodes;
using Xunit;

namespace Senswave.Automations.UnitTests.Conditions.Boolean;

[Trait("Collection", "UnitTests")]
public class ConditionTests : BaseTests
{
    [Fact]
    public void CheckBooleanCondition()
    {
        // Arrange
        var expectTrue = ToBooleanAutomationImplementation(new JsonObject { ["isOn"] = true });
        var expectFalse = ToBooleanAutomationImplementation(new JsonObject { ["isOn"] = false });

        // Act & Assert
        Assert.True(expectTrue.CheckCondition(JsonValue.Create(true)));
        Assert.True(expectFalse.CheckCondition(JsonValue.Create(false)));
        Assert.False(expectTrue.CheckCondition(JsonValue.Create(false)));
        Assert.False(expectFalse.CheckCondition(JsonValue.Create(true)));
        Assert.False(expectTrue.CheckCondition(JsonValue.Create("not a boolean")));
        Assert.False(expectFalse.CheckCondition(JsonValue.Create("not a boolean")));
    }
}