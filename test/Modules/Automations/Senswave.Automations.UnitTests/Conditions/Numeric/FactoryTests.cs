using Senswave.Automations.Domain.Factory;
using System.Text.Json.Nodes;
using Xunit;

namespace Senswave.Automations.UnitTests.Conditions.Numeric;

[Trait("Collection", "UnitTests")]
public class FactoryTests : BaseTests
{
    [Fact]
    public async Task CreateNumericConditionByFactory()
    {
        // Arrange
        var factory = new ConditionFactory();
        var validJson =
            ToNumericAutomationCondition(new JsonObject { ["minValue"] = 37, ["maxValue"] = 38 });
        var inValidJson =
            ToNumericAutomationCondition(new JsonObject { ["minValue"] = "not a number", ["maxValue"] = 38 });

        // Act
        var valid = await factory.Create(validJson, new CancellationToken());
        var inValid = await factory.Create(inValidJson, new CancellationToken());

        // Assert
        Assert.True(valid.IsSuccess);
        Assert.True(inValid.IsFailure);
    }
}