using System.Text.Json.Nodes;
using Xunit;

namespace Senswave.Automations.UnitTests.Conditions.Boolean;

[Trait("Collection", "UnitTests")]
public class ValidationTests : BaseTests
{
    [Fact]
    public async Task ValidateBooleanCondition()
    {
        // Arrange
        var correctTrueJson = ToBooleanAutomationImplementation(new JsonObject { ["isOn"] = true });
        var correctFalseJson = ToBooleanAutomationImplementation(new JsonObject { ["isOn"] = false });

        var inCorrectTrueJson = ToBooleanAutomationImplementation(new JsonObject { ["isOkey"] = true });
        var inCorrectFalseJson = ToBooleanAutomationImplementation(new JsonObject { ["isOkey"] = false });

        // Act
        var valid = await correctTrueJson.Validate(new CancellationToken());
        var valid2 = await correctFalseJson.Validate(new CancellationToken());
        var inValid1 = await inCorrectTrueJson.Validate(new CancellationToken());
        var inValid2 = await inCorrectFalseJson.Validate(new CancellationToken());

        // Assert
        Assert.True(valid.IsSuccess);
        Assert.True(valid2.IsSuccess);
        Assert.False(inValid1.IsSuccess);
        Assert.False(inValid2.IsSuccess);
    }
}