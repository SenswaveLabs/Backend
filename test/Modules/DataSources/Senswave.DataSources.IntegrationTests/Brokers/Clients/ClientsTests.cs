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
using Senswave.Integration.DataSource.BrokerConnection.Logging;
using Senswave.TestInfrastructure.Fixtures.Mqtt;
using Senswave.TestInfrastructure.TestEnvironments.Mqtt;
using System.Text;
using Xunit;

namespace Senswave.DataSources.IntegrationTests.Brokers.Clients;

[Collection("Base Integration Tests Collection")]
[Trait("Collection", "MqttIntegrationTests")]
public class ClientsTests : IClassFixture<MosquittoContainerFixture>
{
    public const int MilisecondSpanBetweenMessagesOnSameTopic = 1000;
    public const int MaximalSizeOfMessageInBytes = 1024;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2211:Pola niebędące stałymi nie powinny być widoczne", Justification = "Test Method")]
    public static IEnumerable<object[]> BrokerData =
    [
        [BrokerProtocolVersion.MqttV5,],
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

    public ClientsTests(MosquittoContainerFixture fixture)
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
    }

    #region Start

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task ClientConnectsToBroker(BrokerProtocolVersion brokerProtocolVersion)
    {
        // Arrange
        _messages = [];
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

        // Act
        var clientResult = _clientFactory.Create(model);
        var client = clientResult.Data!;
        var startResult = await client.Start(_fixture.Username, _fixture.Password, cts.Token)!;

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.True(startResult.IsSuccess);
        Assert.True(client.IsConnected);
        Assert.True(_messages.ContainsKey(nameof(ConnectedLog)));
    }

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task ClientForbidsToConnectIfConnected(BrokerProtocolVersion brokerProtocolVersion)
    {
        // Arrange
        _messages = [];
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

        // Act
        var clientResult = _clientFactory.Create(model);
        var client = clientResult.Data!;
        var startResult = await client.Start(_fixture.Username, _fixture.Password, cts.Token)!;
        var failedStartResult = await client.Start(_fixture.Username, _fixture.Password, cts.Token)!;

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.True(startResult.IsSuccess);
        Assert.True(client.IsConnected);
        Assert.True(_messages.ContainsKey(nameof(ConnectedLog)));
        Assert.False(failedStartResult.IsSuccess);
    }

    #endregion

    #region Stop

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task StoppedClientCannotBeReused(BrokerProtocolVersion brokerProtocolVersion)
    {
        // Arrange
        _messages = [];
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

        // Act
        var clientResult = _clientFactory.Create(model);
        var client = clientResult.Data!;
        var startResult = await client.Start(_fixture.Username, _fixture.Password, cts.Token)!;
        var clientWasConnected = client.IsConnected;
        await Task.Delay(100);
        var stopResult = await client.Stop(cts.Token)!;
        await Task.Delay(100);
        var startTwoResult = await client.Start(_fixture.Username, _fixture.Password, cts.Token)!;
        var restartResult = await client.Restart(cts.Token)!;

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.True(startResult.IsSuccess);
        Assert.True(clientWasConnected);
        Assert.True(stopResult.IsSuccess);
        Assert.False(startTwoResult.IsSuccess);
        Assert.False(restartResult.IsSuccess);
    }

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task ClientCanBeStopped(BrokerProtocolVersion brokerProtocolVersion)
    {
        // Arrange
        _messages = [];
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

        // Act
        var clientResult = _clientFactory.Create(model);
        var client = clientResult.Data!;
        var startResult = await client.Start(_fixture.Username, _fixture.Password, cts.Token)!;
        var clientWasConnected = client.IsConnected;
        await Task.Delay(100);
        var stopResult = await client.Stop(cts.Token)!;
        await Task.Delay(500);

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.True(startResult.IsSuccess);
        Assert.True(clientWasConnected);
        Assert.True(_messages.ContainsKey(nameof(ConnectedLog)));
        Assert.True(stopResult.IsSuccess);
        Assert.False(client.IsConnected);
        Assert.True(_messages.ContainsKey(nameof(DisconnectedLog)));
    }

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task FailsToStopClientIfNotConnected(BrokerProtocolVersion brokerProtocolVersion)
    {
        // Arrange
        _messages = [];
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

        // Act
        var clientResult = _clientFactory.Create(model);
        var client = clientResult.Data!;
        var stopResult = await client.Stop(cts.Token)!;

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.False(client.IsConnected);
        Assert.False(stopResult.IsSuccess);
        Assert.Empty(_messages);
    }

    #endregion

    #region Restart

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task FailsToRestartClientIfConnected(BrokerProtocolVersion brokerProtocolVersion)
    {
        // Arrange
        _messages = [];
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

        // Act
        var clientResult = _clientFactory.Create(model);
        var client = clientResult.Data!;
        var startResult = await client.Start(_fixture.Username, _fixture.Password, cts.Token)!;
        var clientWasConnected = client.IsConnected;
        await Task.Delay(100);
        var restartResult = await client.Restart(cts.Token)!;

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.True(startResult.IsSuccess);
        Assert.True(clientWasConnected);
        Assert.False(restartResult.IsSuccess);
    }

    #endregion

    #region Publish
    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task CanPublishDataToBroker(BrokerProtocolVersion brokerProtocolVersion)
    {
        // Arrange
        _messages = [];
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

        // Act
        var clientResult = _clientFactory.Create(model);
        var client = clientResult.Data!;
        var startResult = await client.Start(_fixture.Username, _fixture.Password, cts.Token)!;
        var published = await client.Publish("test", "test", cts.Token)!;

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.True(startResult.IsSuccess);
        Assert.True(client.IsConnected);
        Assert.True(published.IsSuccess);
    }

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task IsNotPublishingIfBrokerNotWorking(BrokerProtocolVersion brokerProtocolVersion)
    {
        // Arrange
        _messages = [];
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

        // Act
        var clientResult = _clientFactory.Create(model);
        var client = clientResult.Data!;
        var published = await client.Publish("test", "test", cts.Token)!;

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.False(client.IsConnected);
        Assert.False(published.IsSuccess);
    }

    #endregion

    #region Subscribe

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task ConstrutorTopicsAreWorking(BrokerProtocolVersion brokerProtocolVersion)
    {
        // Arrange
        _messages = [];
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        var topic = model.Subscribtions.First();
        var observer = await StartObserverClient(topic, cts.Token);
        var messageReceived = 0;

        observer.ApplicationMessageReceivedAsync += (e) =>
        {
            messageReceived += 1;
            return Task.CompletedTask;
        };

        // Act
        var clientResult = _clientFactory.Create(model);
        var client = clientResult.Data!;
        var startResult = await client.Start(_fixture.Username, _fixture.Password, cts.Token)!;
        var subscribeFirstResult = await client.Subscribe(topic, cts.Token)!;
        var published = await client.Publish(topic, "testData", cts.Token)!;

        int i = 0;
        while (messageReceived == 0 && i < 30)
        {
            await Task.Delay(100);
            i++;
        }

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.True(startResult.IsSuccess);
        Assert.True(subscribeFirstResult.IsSuccess);
        Assert.True(client.IsConnected);
        Assert.True(published.IsSuccess);
        Assert.Equal(1, messageReceived);

        // Cleanup
        await MqttHelpers.Cleanup(observer, cts.Token);
    }

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task SubscribesToTopicAndReceivesData(BrokerProtocolVersion brokerProtocolVersion)
    {
        // Arrange
        _messages = [];
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        var topic = $"testTopicForSend/{Guid.NewGuid()}";
        var observer = await StartObserverClient(topic, cts.Token);
        var messageReceived = 0;

        observer.ApplicationMessageReceivedAsync += (e) =>
        {
            messageReceived += 1;
            return Task.CompletedTask;
        };

        // Act
        var clientResult = _clientFactory.Create(model);
        var client = clientResult.Data!;

        var subscribeFirstResult = await client.Subscribe(topic, cts.Token)!;
        var startResult = await client.Start(_fixture.Username, _fixture.Password, cts.Token)!;
        var published = await client.Publish(topic, "testData", cts.Token)!;

        int i = 0;
        while (messageReceived == 0 && i < 30)
        {
            await Task.Delay(100);
            i++;
        }

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.True(startResult.IsSuccess);
        Assert.True(subscribeFirstResult.IsSuccess);
        Assert.True(client.IsConnected);
        Assert.True(published.IsSuccess);
        Assert.Equal(1, messageReceived);

        // Cleanup
        await MqttHelpers.Cleanup(observer, cts.Token);
    }

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task SubscribesToTopicWhenRunningAndReceivesData(BrokerProtocolVersion brokerProtocolVersion)
    {
        // Arrange
        _messages = [];
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        var topic = $"testTopicForSend/{Guid.NewGuid()}";
        var observer = await StartObserverClient(topic, cts.Token);
        var messageReceived = 0;

        observer.ApplicationMessageReceivedAsync += (e) =>
        {
            messageReceived += 1;
            return Task.CompletedTask;
        };

        // Act
        var clientResult = _clientFactory.Create(model);
        var client = clientResult.Data!;

        var startResult = await client.Start(_fixture.Username, _fixture.Password, cts.Token)!;
        var subscribeFirstResult = await client.Subscribe(topic, cts.Token)!;
        var published = await client.Publish(topic, "testData", cts.Token)!;

        int i = 0;
        while (messageReceived == 0 && i < 30)
        {
            await Task.Delay(100);
            i++;
        }

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.True(startResult.IsSuccess);
        Assert.True(subscribeFirstResult.IsSuccess);
        Assert.True(client.IsConnected);
        Assert.True(published.IsSuccess);
        Assert.Equal(1, messageReceived);

        // Cleanup
        await MqttHelpers.Cleanup(observer, cts.Token);
    }

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task DoesNotResubscirbeTopic(BrokerProtocolVersion brokerProtocolVersion)
    {
        // Arrange
        _messages = [];
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        var topic = "testTopicForSend";

        // Act
        var clientResult = _clientFactory.Create(model);
        var client = clientResult.Data!;

        var subscribeFirstResult = await client.Subscribe(topic, cts.Token)!;
        var subscribeTwoResult = await client.Subscribe(topic, cts.Token)!;

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.False(client.IsConnected);
        Assert.True(subscribeFirstResult.IsSuccess);
        Assert.True(subscribeTwoResult.IsSuccess);
    }

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task DoesNotResubscirbeTopicWhenRunning(BrokerProtocolVersion brokerProtocolVersion)
    {
        // Arrange
        _messages = [];
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        var topic = $"testTopicForSend/{Guid.NewGuid()}";
        var observer = await StartObserverClient(topic, cts.Token);
        var messageReceived = 0;

        observer.ApplicationMessageReceivedAsync += (e) =>
        {
            messageReceived += 1;
            return Task.CompletedTask;
        };

        // Act
        var clientResult = _clientFactory.Create(model);
        var client = clientResult.Data!;

        var startResult = await client.Start(_fixture.Username, _fixture.Password, cts.Token)!;
        var subscribeFirstResult = await client.Subscribe(topic, cts.Token)!;
        var subscribeSecondResult = await client.Subscribe(topic, cts.Token)!;
        var published = await client.Publish(topic, "testData", cts.Token)!;

        int i = 0;
        while (messageReceived == 0 && i < 30)
        {
            await Task.Delay(100);
            i++;
        }

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.True(startResult.IsSuccess);
        Assert.True(client.IsConnected);
        Assert.True(subscribeFirstResult.IsSuccess);
        Assert.True(subscribeSecondResult.IsSuccess);
        Assert.True(published.IsSuccess);
        Assert.Equal(1, messageReceived);

        // Cleanup
        await MqttHelpers.Cleanup(observer, cts.Token);
    }

    #endregion

    #region Unsubscribe

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task UnsubscribesToTopic(BrokerProtocolVersion brokerProtocolVersion)
    {
        // Arrange
        _messages = [];
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        var topic = model.Subscribtions.First();
        var observer = await StartObserverClient(topic, cts.Token);

        // Act
        var clientResult = _clientFactory.Create(model);
        var client = clientResult.Data!;

        var unsubscribeFirstResult = await client.Unsubscribe(topic, cts.Token)!;
        var startResult = await client.Start(_fixture.Username, _fixture.Password, cts.Token)!;
        var published = await observer.PublishStringAsync(topic, "testData", cancellationToken: cts.Token)!;

        await Task.Delay(1000);

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.True(startResult.IsSuccess);
        Assert.True(unsubscribeFirstResult.IsSuccess);
        Assert.True(client.IsConnected);
        Assert.True(published.IsSuccess);
        Assert.False(_messages.ContainsKey(nameof(MessageReceivedEvent)));

        // Cleanup
        await MqttHelpers.Cleanup(observer, cts.Token);
    }

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task UnsubscribesToTopicWhenRunning(BrokerProtocolVersion brokerProtocolVersion)
    {
        // Arrange
        _messages = [];
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        var topic = model.Subscribtions.First();
        var observer = await StartObserverClient(topic, cts.Token);

        // Act
        var clientResult = _clientFactory.Create(model);
        var client = clientResult.Data!;

        var startResult = await client.Start(_fixture.Username, _fixture.Password, cts.Token)!;
        var unsubscribeFirstResult = await client.Unsubscribe(topic, cts.Token)!;
        var published = await observer.PublishStringAsync(topic, "testData", cancellationToken: cts.Token)!;

        await Task.Delay(1000);

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.True(startResult.IsSuccess);
        Assert.True(unsubscribeFirstResult.IsSuccess);
        Assert.True(client.IsConnected);
        Assert.True(published.IsSuccess);
        Assert.False(_messages.ContainsKey(nameof(MessageReceivedEvent)));

        // Cleanup
        await MqttHelpers.Cleanup(observer, cts.Token);
    }

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task FailsToUnsubscirbeIfNotSubscribed(BrokerProtocolVersion brokerProtocolVersion)
    {
        // Arrange
        _messages = [];
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        var topic = model.Subscribtions.First();
        var messageReceived = 0;


        // Act
        var clientResult = _clientFactory.Create(model);
        var client = clientResult.Data!;

        var unsubscribeFirstResult = await client.Unsubscribe(topic, cts.Token)!;
        var unsubscribeSecondResult = await client.Unsubscribe(topic, cts.Token)!;

        int i = 0;
        while (messageReceived == 0 && i < 10)
        {
            await Task.Delay(100);
            i++;
        }

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.True(unsubscribeFirstResult.IsSuccess);
        Assert.False(unsubscribeSecondResult.IsSuccess);
        Assert.Equal(0, messageReceived);
    }

    #endregion

    #region Receive

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task ReceivesDataOnTopic(BrokerProtocolVersion brokerProtocolVersion)
    {
        // Arrange
        _messages = [];
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        var topic = model.Subscribtions.First();
        var observer = await StartObserverClient(topic, cts.Token);

        // Act
        var clientResult = _clientFactory.Create(model);
        var client = clientResult.Data!;

        var startResult = await client.Start(_fixture.Username, _fixture.Password, cts.Token)!;
        var publishedFirst = await observer.PublishStringAsync(topic, "testData", cancellationToken: cts.Token)!;
        await Task.Delay(MilisecondSpanBetweenMessagesOnSameTopic);

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.True(startResult.IsSuccess);
        Assert.True(client.IsConnected);
        Assert.True(publishedFirst.IsSuccess);
        Assert.True(_messages.ContainsKey(nameof(MessageReceivedEvent)));

        // Cleanup
        await MqttHelpers.Cleanup(observer, cts.Token);
    }

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task RateLimitingWorks(BrokerProtocolVersion brokerProtocolVersion)
    {
        // Arrange
        _messages = [];
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        var topic = model.Subscribtions.First();
        var observer = await StartObserverClient(topic, cts.Token);

        _rateLimiterOptionsMock.Setup(x => x.CurrentValue)
            .Returns(new RateLimitersOptions
            {
                MessageRateLimiter = new MessageRateLimiterOptions
                {
                    TokenLimit = 2,
                    KeepTokenSeconds = 60
                }
            });

        // Act
        var clientResult = _clientFactory.Create(model);
        var client = clientResult.Data!;

        var startResult = await client.Start(_fixture.Username, _fixture.Password, cts.Token)!;
        var publishedFirst = await observer.PublishStringAsync(topic, "testData", cancellationToken: cts.Token)!;
        var publishedSecond = await observer.PublishStringAsync(topic, "testData", cancellationToken: cts.Token)!;
        var publishThird = await observer.PublishStringAsync(topic, "testData", cancellationToken: cts.Token)!;

        await Task.Delay(1000);

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.True(startResult.IsSuccess);
        Assert.True(client.IsConnected);
        Assert.True(publishedFirst.IsSuccess);
        Assert.True(publishedSecond.IsSuccess);
        Assert.True(publishThird.IsSuccess);
        Assert.True(_messages.ContainsKey(nameof(MessageReceivedEvent)));
        Assert.Equal(2, _messages[nameof(MessageReceivedEvent)]);

        // Cleanup
        await MqttHelpers.Cleanup(observer, cts.Token);

        _rateLimiterOptionsMock.Setup(x => x.CurrentValue)
            .Returns(new RateLimitersOptions
            {
                MessageRateLimiter = new MessageRateLimiterOptions
                {
                    TokenLimit = 100,
                    KeepTokenSeconds = 60
                }
            });
    }

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task RateLimitingRefreshes(BrokerProtocolVersion brokerProtocolVersion)
    {
        // Arrange
        _messages = [];
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        var topic = model.Subscribtions.First();
        var observer = await StartObserverClient(topic, cts.Token);

        _rateLimiterOptionsMock.Setup(x => x.CurrentValue)
            .Returns(new RateLimitersOptions
            {
                MessageRateLimiter = new MessageRateLimiterOptions
                {
                    TokenLimit = 2,
                    KeepTokenSeconds = 1
                }
            });

        // Act
        var clientResult = _clientFactory.Create(model);
        var client = clientResult.Data!;

        var startResult = await client.Start(_fixture.Username, _fixture.Password, cts.Token)!;
        var publishedFirst = await observer.PublishStringAsync(topic, "testData", cancellationToken: cts.Token)!;
        var publishedSecond = await observer.PublishStringAsync(topic, "testData", cancellationToken: cts.Token)!;
        var publishThird = await observer.PublishStringAsync(topic, "testData", cancellationToken: cts.Token)!;

        await Task.Delay(2000);

        var processedMessages = _messages[nameof(MessageReceivedEvent)];

        var publishFourth = await observer.PublishStringAsync(topic, "testData", cancellationToken: cts.Token)!;

        await Task.Delay(2000);

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.True(startResult.IsSuccess);
        Assert.True(client.IsConnected);
        Assert.True(publishedFirst.IsSuccess);
        Assert.True(publishedSecond.IsSuccess);
        Assert.True(publishThird.IsSuccess);
        Assert.True(publishFourth.IsSuccess);
        Assert.True(_messages.ContainsKey(nameof(MessageReceivedEvent)));
        Assert.Equal(2, processedMessages);
        Assert.Equal(3, _messages[nameof(MessageReceivedEvent)]);

        // Cleanup
        await MqttHelpers.Cleanup(observer, cts.Token);

        _rateLimiterOptionsMock.Setup(x => x.CurrentValue)
            .Returns(new RateLimitersOptions
            {
                MessageRateLimiter = new MessageRateLimiterOptions
                {
                    TokenLimit = 100,
                    KeepTokenSeconds = 60
                }
            });
    }

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task IgnoresTooBigMessages(BrokerProtocolVersion brokerProtocolVersion)
    {
        // Arrange
        _messages = [];
        var model = BaseClientModel(brokerProtocolVersion);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        var topic = model.Subscribtions.First();
        var observer = await StartObserverClient(topic, cts.Token);
        var payload = new string('a', 2*MaximalSizeOfMessageInBytes);

        // Act
        var clientResult = _clientFactory.Create(model);
        var client = clientResult.Data!;

        var startResult = await client.Start(_fixture.Username, _fixture.Password, cts.Token)!;
        var publishedFirst = await observer.PublishStringAsync(topic, payload, cancellationToken: cts.Token)!;
        await Task.Delay(1000);

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.True(startResult.IsSuccess);
        Assert.True(client.IsConnected);
        Assert.True(publishedFirst.IsSuccess);
        Assert.False(_messages.ContainsKey(nameof(MessageReceivedEvent)));

        // Cleanup
        await MqttHelpers.Cleanup(observer, cts.Token);
    }

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task ClientDoesNotReceivesDuplicates(BrokerProtocolVersion brokerProtocolVersion)
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

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.True(startResult.IsSuccess);
        Assert.True(client.IsConnected);
        Assert.True(publishResult.IsSuccess);
        Assert.False(_messages.ContainsKey(nameof(MessageReceivedEvent)), $"Message types count: {_messages.Count}");
        Assert.Single(receivedByObserver);
        Assert.True(receivedByObserver[0].Item2 == payload);

        // Cleanup
        await MqttHelpers.Cleanup(observer, cts.Token);
    }

    [Theory]
    [MemberData(nameof(BrokerData))]
    public async Task ClientDoesNotReceivesDuplicatesButReceivesValidMessages(BrokerProtocolVersion brokerProtocolVersion)
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
        await Task.Delay(200);

        var publishSecondResult = await observer.PublishStringAsync(topic, payload, cancellationToken: cts.Token)!;
        await Task.Delay(1000);

        // Assert
        Assert.True(clientResult.IsSuccess);
        Assert.True(startResult.IsSuccess);
        Assert.True(client.IsConnected);
        Assert.True(publishResult.IsSuccess);
        Assert.True(publishSecondResult.IsSuccess);
        Assert.True(_messages.ContainsKey(nameof(MessageReceivedEvent)));
        Assert.Equal(1, _messages[nameof(MessageReceivedEvent)]);
        Assert.Equal(2, receivedByObserver.Count);
        Assert.True(receivedByObserver[0].Item2 == payload);
        Assert.True(receivedByObserver[1].Item2 == payload);

        // Cleanup
        await MqttHelpers.Cleanup(observer, cts.Token);
    }

    #endregion

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