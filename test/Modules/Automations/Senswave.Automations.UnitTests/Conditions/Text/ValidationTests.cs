using System.Text.Json.Nodes;
using Xunit;

namespace Senswave.Automations.UnitTests.Conditions.Text;

[Trait("Collection", "UnitTests")]
public class ValidationTests : BaseTests
{
    [Fact]
    public async Task ValidateTextCondition()
    {
        // Arrange
        var validJson = ToTextConditionImplementation(new JsonObject { ["requiredValue"] = "(12,13,21)" });
        var inValidJsonKey = ToTextConditionImplementation(new JsonObject { ["mustValue"] = "(12,13,21)" });
        var inValidValueType = ToTextConditionImplementation(new JsonObject { ["mustValue"] = 123 });

        // Act
        var valid = await validJson.Validate(new CancellationToken());
        var inValid1 = await inValidJsonKey.Validate(new CancellationToken());
        var inValid2 = await inValidValueType.Validate(new CancellationToken());

        // Assert
        Assert.True(valid.IsSuccess);
        Assert.False(inValid1.IsSuccess);
        Assert.False(inValid2.IsSuccess);
    }
}