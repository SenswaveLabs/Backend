using Senswave.Automations.Domain.Factory;
using System.Text.Json.Nodes;
using Xunit;

namespace Senswave.Automations.UnitTests.Conditions.Boolean;

[Trait("Collection", "UnitTests")]
public class FactoryTests : BaseTests
{
    [Fact]
    public async Task CreateBooleanConditionByFactory()
    {
        var factory = new ConditionFactory();

        // Arrange
        var correctTrueJson = ToBooleanAutomationCondition(new JsonObject { ["isOn"] = true });
        var inCorrectTrueJson = ToBooleanAutomationCondition(new JsonObject { ["isOn"] = "definitely not true" });

        // Act
        var valid = await factory.Create(correctTrueJson, new CancellationToken());
        var inValid = await factory.Create(inCorrectTrueJson, new CancellationToken());

        // Assert
        Assert.True(valid.IsSuccess);
        Assert.True(inValid.IsFailure);
    }
}