using System.Text.Json.Nodes;
using Xunit;

namespace Senswave.Automations.UnitTests.Conditions.Text;

[Trait("Collection", "UnitTests")]
public class ConditionTests : BaseTests
{
    [Fact]
    public void CheckTextCondition()
    {
        // Arrange
        const string requiredValue = "alamakota";
        var condition = ToTextConditionImplementation(new JsonObject { ["requiredValue"] = requiredValue });

        // Act && Assert
        Assert.True(condition.CheckCondition(JsonValue.Create(requiredValue)));
        Assert.False(condition.CheckCondition(JsonValue.Create("kotalam")));

        Assert.False(condition.CheckCondition(JsonValue.Create(2091)));
    }
}