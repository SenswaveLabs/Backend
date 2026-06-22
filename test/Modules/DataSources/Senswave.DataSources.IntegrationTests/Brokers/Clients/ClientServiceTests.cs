using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MQTTnet;
using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.DataSources.BrokerConnection.Clients;
using Senswave.DataSources.BrokerConnection.Factories;
using Senswave.DataSources.BrokerConnection.Features.Start;
using Senswave.DataSources.BrokerConnection.Features.Terminate;
using Senswave.DataSources.BrokerConnection.RateLimiters;
using Senswave.DataSources.BrokerConnection.RateLimiters.Message;
using Senswave.DataSources.BrokerConnection.Services;
using Senswave.DataSources.Domain.Brokers.Brokers.Enums;
using Senswave.DataSources.Domain.Brokers.Brokers.Options;
using Senswave.DataSources.Domain.Brokers.Clients.Options;
using Senswave.DataSources.Domain.Diagnostics;
using Senswave.Integration.DataSource.BrokerConnection.Logging;
using Senswave.TestInfrastructure.Fixtures.Mqtt;
using Xunit;

namespace Senswave.DataSources.IntegrationTests.Brokers.Clients;

[Collection("Base Integration Tests Collection")]
[Trait("Collection", "MqttIntegrationTests")]
public class ClientServiceTests : IClassFixture<MosquittoContainerFixture>
{
    public const int MilisecondSpanBetweenMessagesOnSameTopic = 1000;
    public const int MaximalSizeOfMessageInBytes = 1024;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2211:Pola niebędące stałymi nie powinny być widoczne", Justification = "Test Method")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "<Pending>")]
    public static IEnumerable<object[]> BrokerData =
    [
        [BrokerProtocolVersion.MqttV5,],
        [BrokerProtocolVersion.MqttV311,],
        [BrokerProtocolVersion.MqttV310,],
    ];

    private readonly IMqttFixture _fixture;

    private readonly ClientFactory _clientFactory;

    private readonly ClientService _clientService;

    private readonly MqttFactory _mqttFactory;

    private readonly Mock<IMessageBus> _messageBusMock;

    private readonly Mock<IOptions<BrokerOptions>> _brokerOptionsMock;

    private readonly Mock<IOptionsMonitor<RateLimitersOptions>> _rateLimiterOptionsMock;

    private readonly Mock<IMediator> _mediatorMock;

    private readonly Mock<ILogger<ClientFactory>> _loggerMock;

    private readonly Mock<ILogger<BaseClient>> _clientLoggerMock;

    private readonly Mock<ILogger<ClientService>> _serviceLoggerMock;

    private readonly Mock<ILogger<MessageRateLimiter>> _rateLimiterLoggerMock;

    private readonly Mock<IDataSourcesActivityProvider> _activityProviderMock;

    private Dictionary<string, int> _messages;

    public ClientServiceTests(MosquittoContainerFixture fixture)
    {
        _fixture = fixture;

        _messages = [];
        _mqttFactory = new();
        _loggerMock = new();
        _clientLoggerMock = new();
        _serviceLoggerMock = new();
        _messageBusMock = new();
        _rateLimiterLoggerMock = new();
        _activityProviderMock = new();

        _rateLimiterOptionsMock = new();

        _rateLimiterOptionsMock
            .Setup(x => x.CurrentValue)
            .Returns(new RateLimitersOptions
            {
                MessageRateLimiter = new MessageRateLimiterOptions
                {
                    TokenLimit = 100,
                    KeepTokenSeconds = 60
                }
            });

        _brokerOptionsMock = new();
        _brokerOptionsMock
            .Setup(x => x.Value)
            .Returns(new BrokerOptions
            {
                Client = new ClientOptions
                {
                    MaximalSizeOfMessageInBytes = MaximalSizeOfMessageInBytes,
                    MilisecondSpanBetweenMessagesOnSameTopic = MilisecondSpanBetweenMessagesOnSameTopic,
                    ReconnectMilisecondSpanBetweenAttempts = 1000,
                    ReconnectAttempts = 3
                },
                IsCluster = false,
            });

        _clientFactory = new ClientFactory(
            _messageBusMock.Object,
            _activityProviderMock.Object,
            _brokerOptionsMock.Object,
            _rateLimiterOptionsMock.Object,
            _loggerMock.Object,
            _clientLoggerMock.Object,
            _rateLimiterLoggerMock.Object,
            _mqttFactory);

        _mediatorMock = new();
        _mediatorMock.Setup(x => x.Publish(It.IsAny<TerminateEvent>(), default))
            .Callback<TerminateEvent, CancellationToken>((message, _) =>
            {
                var type = message.GetType().Name;

                if (_messages.TryGetValue(type, out int count))
                    _messages[type] = count + 1;
                else
                    _messages[type] = 1;
            });

        _messageBusMock.Setup(x => x.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((message, _) =>
            {
                var type = message.GetType().Name;

                if (_messages.TryGetValue(type, out int count))
                    _messages[type] = count + 1;
                else
                    _messages[type] = 1;
            });

        _clientService = new ClientService(_serviceLoggerMock.Object, _mediatorMock.Object, _clientFactory);

        _fixture.InitializeAsync().Wait();
    }

    #region Start

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task StartsClientAndDoesNotAllowsCopies(BrokerProtocolVersion brokerProtocolVersion)
    {
        //Act
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        _messages = [];

        //Arrange
        var startFirstResult = await _clientService.StartClient(model, cts.Token);
        var startSecondResult = await _clientService.StartClient(model, cts.Token);
        var clientResult = _clientService.GetClient(model.BrokerId);

        //Assert
        Assert.True(startFirstResult.IsSuccess);
        Assert.True(startSecondResult.IsFailure);
        Assert.True(clientResult.IsSuccess);
    }

    #endregion

    #region Stop

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task StopsClientAndRemoves(BrokerProtocolVersion brokerProtocolVersion)
    {
        //Act
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        _messages = [];

        //Arrange
        var startFirstResult = await _clientService.StartClient(model, cts.Token);
        var clientResult = _clientService.GetClient(model.BrokerId);
        await Task.Delay(1000);
        var stopResult = await _clientService.StopClient(model.BrokerId, CancellationToken.None);
        var clientSecondResult = _clientService.GetClient(model.BrokerId);

        //Assert
        Assert.True(startFirstResult.IsSuccess);
        Assert.True(stopResult.IsSuccess);
        Assert.True(clientResult.IsSuccess);
        Assert.True(clientSecondResult.IsFailure);
        Assert.True(_messages.ContainsKey(nameof(TerminateEvent)));
    }

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task ServiceDisposesGracefully(BrokerProtocolVersion bpt)
    {
        //Act
        var model = BaseClientModel(bpt);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

        //Arrange
        var startFirstResult = await _clientService.StartClient(model, cts.Token);
        await Task.Delay(1000);
        await _clientService.DisposeAsync();
        await Task.Delay(500);
        var clientResult = _clientService.GetClient(model.BrokerId);
        var startSecondResult = await _clientService.StartClient(model, cts.Token);

        //Assert
        Assert.True(startFirstResult.IsSuccess);
        Assert.True(_messages.ContainsKey(nameof(DisconnectedLog)));
        Assert.True(clientResult.IsFailure);
        Assert.True(startSecondResult.IsFailure);
    }

    #endregion

    #region Restart

    //[Theory]
    //[MemberData(nameof(BrokerData))]
    //public async Task ClientIsRestartedWithoutPassword(BrokerProtocolVersion brokerProtocolVersion)
    //{
    //    //Act
    //    var model = BaseClientModel(brokerProtocolVersion);
    //    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
    //    _messages = [];

    //    //Arrange
    //    var startFirstResult = await _clientService.StartClient(model, cts.Token);
    //    var clientResult = _clientService.GetClient(model.BrokerId);
    //    await Task.Delay(1000);
    //    var connected = clientResult.Data.IsConnected;
    //    var stopResult = await clientResult.Data.Stop(cts.Token);
    //    await Task.Delay(1000);
    //    var restartResult = await _clientService.RestartClient(model.BrokerId, cts.Token);

    //    //Assert
    //    Assert.True(startFirstResult.IsSuccess);
    //    Assert.True(clientResult.IsSuccess);
    //    Assert.True(connected);
    //    Assert.True(stopResult.IsSuccess);
    //    Assert.True(restartResult.IsSuccess);
    //    Assert.True(clientResult.IsSuccess);
    //    Assert.True(clientResult.Data.IsConnected);

    //    //Cleanup
    //    await ClientHelpers.Cleanup(clientResult.Data, cts.Token);
    //}

    #endregion

    #region Privates

    private StartClientModel BaseClientModel(BrokerProtocolVersion brokerProtocolVersion) => new()
    {
        BrokerId = Guid.NewGuid(),
        SessionId = Guid.NewGuid(),

        Server = _fixture.Hostname,
        ClientName = $"TestClient{Guid.NewGuid()}",
        Port = _fixture.Port,
        UseTls = _fixture.UseTls,

        ProtocolVersion = brokerProtocolVersion,
        Password = _fixture.Password,
        Username = _fixture.Username,

        Subscribtions = [$"testTopicForSend/{Guid.NewGuid()}"]
    };

    #endregion
}
