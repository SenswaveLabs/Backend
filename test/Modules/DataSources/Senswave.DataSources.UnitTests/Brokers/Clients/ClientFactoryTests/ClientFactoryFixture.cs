using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MQTTnet;
using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.DataSources.BrokerConnection.Clients;
using Senswave.DataSources.BrokerConnection.Factories;
using Senswave.DataSources.BrokerConnection.Features.Start;
using Senswave.DataSources.BrokerConnection.RateLimiters;
using Senswave.DataSources.BrokerConnection.RateLimiters.Message;
using Senswave.DataSources.Domain.Brokers.Brokers.Enums;
using Senswave.DataSources.Domain.Brokers.Brokers.Options;
using Senswave.DataSources.Domain.Diagnostics;

namespace Senswave.DataSources.UnitTests.Brokers.Clients.ClientFactoryTests;

public class ClientFactoryFixture
{
    private readonly MqttFactory _mqttFactory;

    private readonly Mock<IMessageBus> _messageBusMock;

    private readonly Mock<IOptions<BrokerOptions>> _brokerOptionsMock;

    private readonly Mock<IOptionsMonitor<RateLimitersOptions>> _rateLimiterOptionsMock;

    private readonly Mock<ILogger<ClientFactory>> _loggerMock;

    private readonly Mock<ILogger<BaseClient>> _clientLoggerMock;

    private readonly Mock<ILogger<MessageRateLimiter>> _rateLimiterLoggerMock;

    private readonly Mock<IDataSourcesActivityProvider> _activityProviderMock;

    public readonly ClientFactory ClientFactory;

    public ClientFactoryFixture()
    {
        _mqttFactory = new();
        _loggerMock = new();
        _clientLoggerMock = new();
        _messageBusMock = new();
        _brokerOptionsMock = new();
        _rateLimiterOptionsMock = new();
        _rateLimiterLoggerMock = new();
        _activityProviderMock = new();

        _brokerOptionsMock
            .Setup(x => x.Value)
            .Returns(new BrokerOptions());

        ClientFactory = new ClientFactory(
            _messageBusMock.Object,
            _activityProviderMock.Object,
            _brokerOptionsMock.Object,
            _rateLimiterOptionsMock.Object,
            _loggerMock.Object,
            _clientLoggerMock.Object,
            _rateLimiterLoggerMock.Object,
            _mqttFactory);
    }

    #region Privates

    public StartClientModel BaseClientModel(BrokerProtocolVersion brokerProtocolVersion) => new()
    {
        BrokerId = Guid.NewGuid(),
        SessionId = Guid.NewGuid(),

        Server = "localhost",
        ClientName = "TestClient",
        Port = 1883,
        UseTls = false,

        ProtocolVersion = brokerProtocolVersion,
        Password = "MqttPassword",
        Username = "MqttUsername",

        Subscribtions = ["test"]
    };

    #endregion
}
