using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Senswave.Abstractions.Contexts;
using Senswave.Abstractions.Resulting;
using Senswave.Users.Domain.Interfaces;
using Senswave.Users.Infrastructure.Middlewares;

namespace Senswave.Users.UnitTests;

public class LegalMiddlewareTests
{
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ILogger<LegalMiddleware>> _loggerMock;

    private readonly DefaultHttpContext _httpContext;
    private readonly RequestDelegate _nextDelegate;

    public LegalMiddlewareTests()
    {
        _requestContextMock = new Mock<IRequestContext>();
        _userServiceMock = new Mock<IUserService>();
        _loggerMock = new Mock<ILogger<LegalMiddleware>>();

        _httpContext = new DefaultHttpContext();
        _nextDelegate = context => Task.CompletedTask;
    }

    [Theory]
    [InlineData("/api/v1/brokers")]
    [InlineData("/api/v1/clients")]
    [InlineData("/api/v1/sessions")]
    [InlineData("/api/v1/homes")]
    [InlineData("/api/v1/rooms")]
    [InlineData("/api/v1/devices")]
    [InlineData("/api/v1/dashboards")]
    [InlineData("/api/v1/operations")]
    [InlineData("/api/v1/widgets")]
    [InlineData("/api/v1/automations")]
    public async Task CheckForConsent(string path)
    {
        // Arrange
        _httpContext.Request.Path = path;

        _requestContextMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _userServiceMock.Setup(x =>
                x.UserHasLatestConsents(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure());

        var middleware = new LegalMiddleware(
            _requestContextMock.Object,
            _userServiceMock.Object,
            _loggerMock.Object);

        // Act
        await middleware.InvokeAsync(_httpContext, _nextDelegate);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, _httpContext.Response.StatusCode);
    }

    [Theory]
    [InlineData("/api/v1/brokers")]
    [InlineData("/api/v1/clients")]
    [InlineData("/api/v1/sessions")]
    [InlineData("/api/v1/homes")]
    [InlineData("/api/v1/rooms")]
    [InlineData("/api/v1/devices")]
    [InlineData("/api/v1/dashboards")]
    [InlineData("/api/v1/operations")]
    [InlineData("/api/v1/widgets")]
    [InlineData("/api/v1/automations")]
    public async Task InvalidUserIdSkipsLegal(string path)
    {
        // Arrange
        _httpContext.Request.Path = path;

        _requestContextMock.Setup(x => x.UserId).Returns(Guid.Empty);
        _userServiceMock.Setup(x =>
                x.UserHasLatestConsents(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                Assert.Fail();

                return Task.FromResult(Result.Success());
            });

        var nextCalled = false;
        RequestDelegate next = context =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new LegalMiddleware(
            _requestContextMock.Object,
            _userServiceMock.Object,
            _loggerMock.Object);

        // Act
        await middleware.InvokeAsync(_httpContext, next);

        // Assert
        Assert.True(nextCalled);
        Assert.NotEqual(StatusCodes.Status403Forbidden, _httpContext.Response.StatusCode);
    }


    [Theory]
    [InlineData("/api/v1/auth")]
    [InlineData("/api/v1/users")]
    [InlineData("/api/v1/legal")]
    [InlineData("/api/health")]
    [InlineData("/.well-known/apple-app-site-association")]
    [InlineData("/.well-known/assetlinks.json")]
    public async Task RequiredPathsAsync(string path)
    {
        // Arrange
        _httpContext.Request.Path = path;

        _requestContextMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _userServiceMock.Setup(x =>
                x.UserHasLatestConsents(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                Assert.Fail();

                return Task.FromResult(Result.Success());
            });

        var nextCalled = false;
        RequestDelegate next = context =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new LegalMiddleware(
            _requestContextMock.Object,
            _userServiceMock.Object,
            _loggerMock.Object);

        // Act
        await middleware.InvokeAsync(_httpContext, next);

        // Assert
        Assert.True(nextCalled);
        Assert.NotEqual(StatusCodes.Status403Forbidden, _httpContext.Response.StatusCode);
    }
}
