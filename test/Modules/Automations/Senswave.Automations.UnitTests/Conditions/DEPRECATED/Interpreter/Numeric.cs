//using Senswave.Automations.Domain.Types.Condition;
//using Senswave.Automations.Domain.Types.Condition.NumericCondition;
//using Xunit;

//namespace Senswave.Automations.UnitTests.Automations.Interpreter;

//[Trait("Collection", "UnitTests")]
//public class Numeric
//{
//    [Fact]
//    public void ShouldBeInDefinedRange()
//    {
//        // Arrange
//        BaseCondition condition = new NumericCondition
//        {
//            Max = 100,
//            Min = 1
//        };

//        var below = "0";
//        var lowBoundary = "1";
//        var inBounds = "46";
//        var highBoundary = "100";
//        var above = "101";

//        // Act
//        var interpret1 = condition.Interpret(below);
//        var interpret2 = condition.Interpret(lowBoundary);
//        var interpret3 = condition.Interpret(inBounds);
//        var interpret4 = condition.Interpret(highBoundary);
//        var interpret5 = condition.Interpret(above);

//        // Assert
//        Assert.False(interpret1);
//        Assert.True(interpret2);
//        Assert.True(interpret3);
//        Assert.True(interpret4);
//        Assert.False(interpret5);
//    }
//}

namespace Senswave.Automations.UnitTests.Conditions.DEPRECATED.Interpreter;