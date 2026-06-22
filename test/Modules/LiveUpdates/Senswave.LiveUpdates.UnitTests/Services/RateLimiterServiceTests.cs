using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Senswave.LiveUpdates.Api;
using Senswave.LiveUpdates.Api.Options;
using Senswave.LiveUpdates.Api.Services.RateLimiter;

namespace Senswave.LiveUpdates.UnitTests.Services;

[Trait("Collection", "UnitTests")]
public class RateLimiterServiceTests
{
    private const int Tokens = 5;
    private const int PeriodSeconds = 10;

    private readonly RateLimitingService rateLimiterService;

    public RateLimiterServiceTests()
    {
        var loggerMock = new Mock<ILogger<RateLimitingService>>();
        var optionsMock = new Mock<IOptions<LiveUpdatesOptions>>();
        var options = new LiveUpdatesOptions
        {
            RateLimiter = new RateLimiterOptions
            {
                ReplenishmentPeriodSeconds = PeriodSeconds,
                TokensPerPeriod = Tokens
            }
        };

        optionsMock.Setup(o => o.Value).Returns(options);

        rateLimiterService = new RateLimitingService(optionsMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task InitialRequestWorks()
    {
        // Arrange
        var connectionId = "test-connection";

        // Act
        var result = await rateLimiterService.RateLimitingAllowsToWork(connectionId);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task LimitIsReached()
    {
        // Arrange
        var connectionId = "test-connection-2";

        // Act
        for (int i = 0; i < Tokens; i++)
        {
            var tmpRes = await rateLimiterService.RateLimitingAllowsToWork(connectionId);

            if (tmpRes.IsFailure)
                Assert.Fail("Unexpected failure during initial requests.");
        }

        var result = await rateLimiterService.RateLimitingAllowsToWork(connectionId);

        // Arrange
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task LimitIsResetAfterPeriod()
    {
        // Arrange
        var connectionId = "test-connection-3";

        // Act
        for (int i = 0; i < Tokens; i++)
        {
            await rateLimiterService.RateLimitingAllowsToWork(connectionId);
        }

        // Wait for the period to reset
        await Task.Delay(TimeSpan.FromSeconds(PeriodSeconds + 1));

        var result = await rateLimiterService.RateLimitingAllowsToWork(connectionId);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task MultipleConnectionsWorkIndependently()
    {
        // Arrange
        var connectionId1 = "test-connection-4";
        var connectionId2 = "test-connection-5";

        // Act
        for (int i = 0; i < Tokens-2; i++)
        {
            var tmpRes = await rateLimiterService.RateLimitingAllowsToWork(connectionId1);

            if (tmpRes.IsFailure)
                Assert.Fail("Unexpected failure during initial requests for connection 1.");

            var tmpRes2 = await rateLimiterService.RateLimitingAllowsToWork(connectionId2);

            if (tmpRes2.IsFailure)
                Assert.Fail("Unexpected failure during initial requests for connection 2.");
        }

        var result1 = await rateLimiterService.RateLimitingAllowsToWork(connectionId1);
        var result2 = await rateLimiterService.RateLimitingAllowsToWork(connectionId2);

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
    }

    [Fact]
    public async Task ConcurrentlyWorksCorrectly()
    {
        // Arrange
        var connectionId = "test-connection-6";

        // Act
        for (int i = 0; i < Tokens; i++)
            _ = rateLimiterService.RateLimitingAllowsToWork(connectionId);

        var result = await rateLimiterService.RateLimitingAllowsToWork(connectionId);

        // Arrange
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ConcurrentlyWorksCorrectlyForMultipleUsers()
    {
        // Arrange
        var connectionId = "test-connection-7";
        var connectionId2 = "test-connection-8";
        var connectionId3 = "test-connection-9";

        // Act
        for (int i = 0; i < Tokens; i++)
        {
            _ = rateLimiterService.RateLimitingAllowsToWork(connectionId);
            _ = rateLimiterService.RateLimitingAllowsToWork(connectionId2);
            _ = rateLimiterService.RateLimitingAllowsToWork(connectionId3);
        }

        var result = await rateLimiterService.RateLimitingAllowsToWork(connectionId);
        var result2 = await rateLimiterService.RateLimitingAllowsToWork(connectionId2);
        var result3 = await rateLimiterService.RateLimitingAllowsToWork(connectionId3);

        // Arrange
        Assert.False(result.IsSuccess);
        Assert.False(result2.IsSuccess);
        Assert.False(result3.IsSuccess);
    }
}
