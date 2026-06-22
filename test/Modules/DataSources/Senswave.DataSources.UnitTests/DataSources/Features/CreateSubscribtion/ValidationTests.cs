using Senswave.DataSources.Application.Brokers.Brokers.Features.CreateSubscribtion;

namespace Senswave.DataSources.UnitTests.DataSources.Features.CreateSubscribtion;

[Trait("Collection", "UnitTests")]
public class ValidationTests
{
    [Theory]
    [InlineData("topic")]
    [InlineData("topic/topic")]
    [InlineData("topic/#")]
    [InlineData("topic/+/topic")]
    [InlineData("ASDF/+/+/ADSF")]
    [InlineData("ASDF/++/ADSF")]
    public void ValidTopic(string topic)
    {
        // Arrange
        var validator = new CreateSubscribtionValidator();
        var command = new CreateSubscribtionCommand
        {
            BrokerId = Guid.NewGuid(),
            Topic = topic
        };

        // Act 
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void TooLongTopic()
    {
        // Arrange
        var validator = new CreateSubscribtionValidator();
        var command = new CreateSubscribtionCommand
        {
            BrokerId = Guid.NewGuid(),
            Topic = new string('a', 70000)
        };

        // Act 
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("$SYS")]
    [InlineData("#")]
    [InlineData("#ASDF")]
    [InlineData("ASDF/#ASFD")]
    [InlineData("ASDF/#/ASFD")]
    [InlineData("+")]
    [InlineData("+/ADSF")]
    public void InvalidTopic(string topic)
    {
        // Arrange
        var validator = new CreateSubscribtionValidator();
        var command = new CreateSubscribtionCommand
        {
            BrokerId = Guid.NewGuid(),
            Topic = topic
        };

        // Act 
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
    }
}
