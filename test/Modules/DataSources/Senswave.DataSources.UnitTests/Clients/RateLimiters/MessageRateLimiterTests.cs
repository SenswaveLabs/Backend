using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Senswave.DataSources.BrokerConnection.RateLimiters;
using Senswave.DataSources.BrokerConnection.RateLimiters.Message;

namespace Senswave.DataSources.UnitTests.Clients.RateLimiters;

[Trait("Collection", "UnitTests")]
public class MessageRateLimiterTests
{
    private readonly Mock<ILogger<MessageRateLimiter>> _loggerMock = new();
    private readonly Mock<IOptionsMonitor<RateLimitersOptions>> _optionsMock = new();

    private MessageRateLimiter CreateLimiter(int tokenLimit = 100, int keepTokenSeconds = 60)
    {
        _optionsMock.Setup(o => o.CurrentValue).Returns(new RateLimitersOptions
        {
            MessageRateLimiter = new MessageRateLimiterOptions
            {
                TokenLimit = tokenLimit,
                KeepTokenSeconds = keepTokenSeconds
            }
        });

        return new MessageRateLimiter(_loggerMock.Object, _optionsMock.Object);
    }

    [Fact]
    public async Task AllowsUpToLimit()
    {
        // Arrange
        var limiter = CreateLimiter(tokenLimit: 100);

        // Act
        int allowedCount = 0;
        for (int i = 0; i < 150; i++)
        {
            if (await limiter.CanProcessMessage(Guid.NewGuid(), Guid.NewGuid(), "test/topic"))
            {
                allowedCount++;
            }
        }

        // Assert
        Assert.Equal(100, allowedCount);
    }

    [Fact]
    public async Task BlocksMessagesOverLimit()
    {
        // Arrange
        var limiter = CreateLimiter(tokenLimit: 5);

        // Act
        for (int i = 0; i < 5; i++)
            await limiter.CanProcessMessage(Guid.NewGuid(), Guid.NewGuid(), "test/topic");

        var allowed = await limiter.CanProcessMessage(Guid.NewGuid(), Guid.NewGuid(), "test/topic");

        // Assert
        Assert.False(allowed);
    }

    [Fact]
    public async Task ConcurrentWritesSafely()
    {
        // Arrange
        var limiter = CreateLimiter(tokenLimit: 100);
        var tasks = Enumerable.Range(0, 200).Select(_ => Task.Run(async () =>
        {
            return await limiter.CanProcessMessage(Guid.NewGuid(), Guid.NewGuid(), "test/topic");
        }));

        // Act
        var results = await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(100, results.Count(r => r));
        Assert.Equal(100, results.Count(r => !r));
    }

    [Fact]
    public async Task ResetAfterTimeWindow()
    {
        // Arrange
        var limiter = CreateLimiter(tokenLimit: 5, keepTokenSeconds: 1);

        for (int i = 0; i < 5; i++)
            await limiter.CanProcessMessage(Guid.NewGuid(), Guid.NewGuid(), "test/topic");

        var blocked = await limiter.CanProcessMessage(Guid.NewGuid(), Guid.NewGuid(), "test/topic");
        Assert.False(blocked);

        // Wait for tokens to expire
        await Task.Delay(1100);

        var allowedAgain = await limiter.CanProcessMessage(Guid.NewGuid(), Guid.NewGuid(), "test/topic");
        Assert.True(allowedAgain);
    }
}
