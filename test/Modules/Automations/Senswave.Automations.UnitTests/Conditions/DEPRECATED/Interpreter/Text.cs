//using Senswave.Automations.Domain.Types.Condition;
//using Senswave.Automations.Domain.Types.Condition.TextCondition;
//using Xunit;

//namespace Senswave.Automations.UnitTests.Automations.Interpreter;

//[Trait("Collection", "UnitTests")]
//public class Text
//{
//    [Fact]
//    public void ShouldMatchWithExpectedValue()
//    {
//        // Arrange
//        var testValue = "Test-value";
//        BaseCondition condition = new TextCondition
//        {
//            RequiredValue = testValue
//        };

//        // Act
//        var interpret1 = condition.Interpret(testValue);
//        var interpret2 = condition.Interpret(testValue + "1");


//        // Assert
//        Assert.True(interpret1);
//        Assert.False(interpret2);
//    }
//}

namespace Senswave.Automations.UnitTests.Conditions.DEPRECATED.Interpreter;