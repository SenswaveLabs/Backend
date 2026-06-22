using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.DataSources.BrokerConnection.Clients;
using Senswave.DataSources.BrokerConnection.Factories;
using Senswave.DataSources.BrokerConnection.Features.Start;
using Senswave.DataSources.BrokerConnection.RateLimiters;
using Senswave.DataSources.BrokerConnection.RateLimiters.Message;
using Senswave.DataSources.Domain.Brokers.Brokers.Enums;
using Senswave.DataSources.Domain.Brokers.Brokers.Options;
using Senswave.DataSources.Domain.Brokers.Clients.Options;
using Senswave.DataSources.Domain.Diagnostics;
using Senswave.Integration.DataSource.BrokerConnection.Events;
using Senswave.TestInfrastructure.Fixtures.Mqtt;
using Senswave.TestInfrastructure.TestEnvironments.Mqtt;
using System.Text;
using Xunit;

namespace Senswave.DataSources.IntegrationTests.Brokers.Clients;

[Collection("Base Integration Tests Collection")]
[Trait("Collection", "MqttIntegrationTests")]
public class MqttV3ClientsTests : IClassFixture<MosquittoContainerFixture>
{
    public const int MilisecondSpanBetweenMessagesOnSameTopic = 1000;
    public const int MaximalSizeOfMessageInBytes = 1024;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2211:Pola niebędące stałymi nie powinny być widoczne", Justification = "Test Method")]
    public static IEnumerable<object[]> LightBrokerData =
[
        [BrokerProtocolVersion.MqttV311,],
        [BrokerProtocolVersion.MqttV310,],
    ];

    private readonly IMqttFixture _fixture;

    private readonly ClientFactory _clientFactory;
    private readonly MqttFactory _mqttFactory;

    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly Mock<IOptions<BrokerOptions>> _brokerOptionsMock;
    private readonly Mock<ILogger<ClientFactory>> _loggerMock;
    private readonly Mock<ILogger<BaseClient>> _clientLoggerMock;
    private readonly Mock<IOptionsMonitor<RateLimitersOptions>> _rateLimiterOptionsMock;
    private readonly Mock<ILogger<MessageRateLimiter>> _rateLimiterLoggerMock;
    private readonly Mock<IDataSourcesActivityProvider> _activityProviderMock;

    private Dictionary<string, int> _messages;

    public MqttV3ClientsTests(MosquittoContainerFixture fixture)
    {
        _fixture = fixture;

        _messages = [];
        _mqttFactory = new();
        _loggerMock = new();
        _clientLoggerMock = new();
        _rateLimiterOptionsMock = new();
        _activityProviderMock = new();

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

        _rateLimiterLoggerMock = new();

        _messageBusMock = new();
        _messageBusMock.Setup(x => x.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((message, _) =>
            {
                var type = message.GetType().Name;

                if (_messages.TryGetValue(type, out int count))
                    _messages[type] = count + 1;
                else
                    _messages[type] = 1;
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
                    ReconnectAttempts = 3,
                    MqttV3 = new MqttV3Options
                    {
                        MessageTimeoutMiliseconds = 1000
                    }
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
    }

    [Theory]
    [MemberData(nameof(LightBrokerData))]
    public async Task LongAwaitingDuplicateIsIgnored(BrokerProtocolVersion brokerProtocolVersion)
    {
        // Arrange
        _messages = [];
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        var topic = model.Subscribtions.First();
        var observer = await StartObserverClient(topic, cts.Token);
        var payload = new string('a', 512);

        var receivedByObserver = new List<(string, string)>();
        observer.ApplicationMessageReceivedAsync += (e) =>
        {
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
            receivedByObserver.Add((e.ApplicationMessage.Topic, payload));
            return Task.CompletedTask;
        };

        // Act
        var clientResult = _clientFactory.Create(model);
        var client = clientResult.Data!;

        var startResult = await client.Start(_fixture.Username, _fixture.Password, cts.Token)!;
        var publishResult = await client.Publish(topic, payload, cts.Token)!;
        await Task.Delay(2000);

        var publishSecondResult = await observer.PublishStringAsync(topic, payload, cancellationToken: cts.Token)!;
        await Task.Delay(2000);

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.True(startResult.IsSuccess);
        Assert.True(client.IsConnected);
        Assert.True(publishResult.IsSuccess);
        Assert.True(publishSecondResult.IsSuccess);
        Assert.True(_messages.ContainsKey(nameof(MessageReceivedEvent)));
        Assert.True(_messages[nameof(MessageReceivedEvent)] == 1, "Message was not received");
        Assert.Equal(2, receivedByObserver.Count);
        Assert.True(receivedByObserver[0].Item2 == payload);
        Assert.True(receivedByObserver[1].Item2 == payload);

        // Cleanup
        await MqttHelpers.Cleanup(observer, cts.Token);
    }

    #region Privates

    private async Task<IMqttClient> StartObserverClient(string topic, CancellationToken cancellationToken)
    {
        // Arrange
        var mqttClient = _mqttFactory.CreateMqttClient();
        var mqttOptionsBuilder = new MqttClientOptionsBuilder()
            .WithCleanSession(true)
            .WithTcpServer(_fixture.Hostname, _fixture.Port)
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(120))
            .WithProtocolVersion(MqttProtocolVersion.V500)
            .WithCredentials(_fixture.Username, _fixture.Password)
            .WithClientId($"TestClient{Guid.NewGuid()}")
            .WithTlsOptions(new MqttClientTlsOptions
            {
                UseTls = _fixture.UseTls
            });

        var topicBuilder = new MqttTopicFilterBuilder()
            .WithAtMostOnceQoS()
            .WithTopic(topic);

        // Act
        await mqttClient.ConnectAsync(mqttOptionsBuilder.Build(), cancellationToken);
        await mqttClient.SubscribeAsync(topicBuilder.Build(), cancellationToken);

        // Assert
        Assert.True(mqttClient.IsConnected);

        return mqttClient;
    }

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
