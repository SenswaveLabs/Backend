//using Senswave.Automations.Domain.Types.Condition;
//using Senswave.Automations.Domain.Types.Condition.BooleanCondition;
//using Xunit;

//namespace Senswave.Automations.UnitTests.Automations.Interpreter;

//[Trait("Collection", "UnitTests")]
//public class Boolean
//{

//    [Fact]
//    public void OperationShouldBeOn()
//    {
//        // Arrange    
//        BaseCondition condition = new BooleanCondition
//        {
//            IsOn = true
//        };

//        var on = "true";
//        var off = "false";
//        var random = "XD";

//        // Act
//        var interpret1 = condition.Interpret(on);
//        var interpret2 = condition.Interpret(off);
//        var interpret3 = condition.Interpret(random);

//        // Assert
//        Assert.True(interpret1);
//        Assert.False(interpret2);
//        Assert.False(interpret3);
//    }

//    [Fact]
//    public void OperationShouldBeOff()
//    {
//        // Arrange    
//        BaseCondition condition = new BooleanCondition
//        {
//            IsOn = false
//        };

//        var on = "true";
//        var off = "false";
//        var random = "XD";

//        // Act
//        var interpret1 = condition.Interpret(on);
//        var interpret2 = condition.Interpret(off);
//        var interpret3 = condition.Interpret(random);

//        // Assert
//        Assert.False(interpret1);
//        Assert.True(interpret2);
//        Assert.False(interpret3);
//    }


//}

namespace Senswave.Automations.UnitTests.Conditions.DEPRECATED.Interpreter;