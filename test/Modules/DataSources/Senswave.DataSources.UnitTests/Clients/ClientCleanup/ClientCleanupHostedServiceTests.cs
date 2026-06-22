using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Senswave.Abstractions.Resulting;
using Senswave.DataSources.BrokerConnection.Features.Cleanup;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.DataSources.Domain.Brokers.Clients.Entities;
using Senswave.DataSources.Domain.Brokers.Clients.Options;

namespace Senswave.DataSources.UnitTests.Clients.ClientCleanup;

[Trait("Collection", "UnitTests")]
public class ClientCleanupHostedServiceTests
{
    private readonly Mock<IServiceScope> _serviceScope;
    private readonly Mock<IServiceProvider> _internalServiceProvider;
    private readonly Mock<ILogger<CleanupHostedService>> _logger;
    private readonly Mock<ILogger<CleanupService>> _cleanupLogger;
    private readonly Mock<IClientService> _clientServiceMock;
    private readonly Mock<IOptions<ClientCleanupOptions>> _clientCleanupOptions;
    private readonly Mock<IGracefulStopClientService> _gracefulStopClientService;

    private readonly ICleanupService _cleanupService;
    private readonly CleanupHostedService _worker;

    private const double LoopDelaySeconds = 1;

    public ClientCleanupHostedServiceTests()
    {
        _serviceScope = new();
        _internalServiceProvider = new();
        _logger = new();
        _cleanupLogger = new();
        _clientServiceMock = new();
        _clientCleanupOptions = new();

        _clientCleanupOptions.Setup(x => x.Value)
            .Returns(new ClientCleanupOptions
            {
                CleanupSpanMinutes = LoopDelaySeconds/60,
                ClientCleanupSpanMiliseconds = 1000
            });

        _serviceScope
            .Setup(x => x.ServiceProvider)
            .Returns(_internalServiceProvider.Object);

        _cleanupService = new CleanupService(
            _clientServiceMock.Object,
            _clientCleanupOptions.Object,
            _cleanupLogger.Object);

        _gracefulStopClientService = new();

        _worker = new CleanupHostedService(
            _gracefulStopClientService.Object,
            _cleanupService,
            _clientCleanupOptions.Object,
            _logger.Object);
    }

    [Fact]
    public async Task WorkerIsNotRemovingWorkingConnections()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var requestedConnectionExistance = false;
        var isRemoved = false;
        var mockClient = new Mock<IClient>();
        var clientList = new List<Guid>
        {
            guid
        };

        _clientServiceMock.Setup(x => x.GetClientIds())
            .Returns(Result<List<Guid>>.Success(clientList) as Result<List<Guid>>);

        mockClient.Setup(x => x.IsConnected)
            .Returns(true);

        _clientServiceMock
            .Setup(x => x.GetClient(guid))
            .Returns(() =>
            {
                requestedConnectionExistance = true;
                return Result<IClient>.Success(mockClient.Object)  as Result<IClient>;
            });

        _clientServiceMock.Setup(x => x.StopClient(guid, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                isRemoved = true;
                return Result<Guid>.Success(guid) as Result<Guid>;
            });

        var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(1)).Token;

        // Act
        await _worker.RemoveOutdatedClients(cancellationToken);

        // Assert
        Assert.True(requestedConnectionExistance);
        Assert.False(isRemoved);
    }

    [Fact]
    public async Task WorkerRemovesConnections()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var requestedConnectionExistance = false;
        var isRemoved = false;
        var mockClient = new Mock<IClient>();
        var clientList = new List<Guid>
        {
            guid
        };

        _clientServiceMock.Setup(x => x.GetClientIds())
            .Returns(Result<List<Guid>>.Success(clientList) as Result<List<Guid>>);

        mockClient.Setup(x => x.Remove)
            .Returns(true);

        _clientServiceMock
            .Setup(x => x.GetClient(guid))
            .Returns(() =>
            {
                requestedConnectionExistance = true;
                return Result<IClient>.Success(mockClient.Object)  as Result<IClient>;
            });

        _clientServiceMock.Setup(x => x.StopClient(guid, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                isRemoved = true;
                return Result<Guid>.Success(guid)  as Result<Guid>;
            });

        var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(1)).Token;

        // Act
        await _worker.RemoveOutdatedClients(cancellationToken);

        // Assert
        Assert.True(requestedConnectionExistance);
        Assert.True(isRemoved, "Failed to stop client");
    }

    [Fact]
    public async Task WorkerIsNotRemovingConnectionsTooFast()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var requestedConnectionExistance = false;
        var requestedOnce = false;
        var requestedTwice = false;
        var requestedTwiceAt = DateTime.MaxValue;
        var removed = false;
        var mockClient = new Mock<IClient>();
        var clientList = new List<Guid>
        {
            guid
        };

        _clientServiceMock.Setup(x => x.GetClientIds())
            .Returns(Result<List<Guid>>.Success(clientList) as Result<List<Guid>>);

        mockClient.Setup(x => x.Remove)
            .Returns(() =>
            {
                if (requestedOnce)
                {
                    requestedTwice = true;
                    requestedTwiceAt = DateTime.UtcNow;
                    return true;
                }

                requestedOnce = true;
                return false;
            });

        _clientServiceMock
            .Setup(x => x.GetClient(guid))
            .Returns(() =>
            {
                requestedConnectionExistance = true;
                return Result<IClient>.Success(mockClient.Object) as Result<IClient>;
            });

        _clientServiceMock.Setup(x => x.StopClient(guid, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                removed = true;
                return Result<Guid>.Success(guid)  as Result<Guid>;
            });

        var delay = 4;

        var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(delay)).Token;

        // Act
        var startedAt = DateTime.UtcNow;
        await _worker.RemoveOutdatedClients(cancellationToken);

        // Assert
        Assert.False(requestedTwiceAt==DateTime.MaxValue);
        Assert.True(requestedConnectionExistance);
        Assert.True(requestedOnce);
        Assert.True(requestedTwice);
        Assert.True(removed);

        Assert.True(requestedTwiceAt - startedAt > TimeSpan.FromSeconds(LoopDelaySeconds));
    }
}
