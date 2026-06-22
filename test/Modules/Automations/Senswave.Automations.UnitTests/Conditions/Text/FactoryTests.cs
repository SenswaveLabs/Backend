using Senswave.Automations.Domain.Factory;
using System.Text.Json.Nodes;
using Xunit;

namespace Senswave.Automations.UnitTests.Conditions.Text;

[Trait("Collection", "UnitTests")]
public class FactoryTests : BaseTests
{
    [Fact]
    public async Task CreateTextConditionByFactory()
    {
        // Arrange
        var factory = new ConditionFactory();
        var validJson =
            ToTextAutomationCondition(new JsonObject { ["requiredValue"] = "valid text" });
        var inValidJson =
            ToTextAutomationCondition(new JsonObject { ["textValue"] = 12345 });

        // Act
        var valid = await factory.Create(validJson, new CancellationToken());
        var inValid = await factory.Create(inValidJson, new CancellationToken());

        // Assert
        Assert.True(valid.IsSuccess);
        Assert.True(inValid.IsFailure);
    }
}