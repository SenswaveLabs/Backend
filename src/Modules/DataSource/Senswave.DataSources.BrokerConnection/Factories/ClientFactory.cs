using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.Abstractions.Resulting;
using Senswave.DataSources.BrokerConnection.Clients;
using Senswave.DataSources.BrokerConnection.Clients.MqttV3;
using Senswave.DataSources.BrokerConnection.Features.Start;
using Senswave.DataSources.BrokerConnection.RateLimiters;
using Senswave.DataSources.BrokerConnection.RateLimiters.Message;
using Senswave.DataSources.Domain.Brokers.Brokers.Enums;
using Senswave.DataSources.Domain.Brokers.Brokers.Extensions;
using Senswave.DataSources.Domain.Brokers.Brokers.Options;
using Senswave.DataSources.Domain.Brokers.Clients.Entities;
using Senswave.DataSources.Domain.Brokers.Clients.Options;
using Senswave.DataSources.Domain.Diagnostics;

namespace Senswave.DataSources.BrokerConnection.Factories;

public class ClientFactory(
    IMessageBus messageBus,
    IDataSourcesActivityProvider activityProvider,
    IOptions<BrokerOptions> options,
    IOptionsMonitor<RateLimitersOptions> rateLimiterOptions,
    ILogger<ClientFactory> logger,
    ILogger<BaseClient> baseClientLogger,
    ILogger<MessageRateLimiter> rateLimiterLogger,
    MqttFactory factory)
{
    #region Errors

    private static readonly Error FailedToCreateClient = Error.Failure("FailedToCreateClient", "Failed to create MQTT client.");
    private static readonly Error ClientHasInvalidConfigration = Error.Failure("ClientHasInvalidConfigration", "Client has an invalid configuration.");

    #endregion

    public Result<IClient> Create(StartClientModel data)
    {
        if (!data.ProtocolVersion.IsValidForNetClient())
        {
            logger.LogError("[BrokerId: {brokerid}][SessionId: {sessionId}] Client has invaid protocol version.",
                data.BrokerId,
                data.SessionId);

            return Result<IClient>.Failure([ClientHasInvalidConfigration]);
        }

        var mqttOptionsBuilder = new MqttClientOptionsBuilder();

        mqttOptionsBuilder.WithCleanSession(true)
            .WithClientId(data.ClientName)
            .WithTcpServer(data.Server, data.Port)
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(120))
            .WithoutPacketFragmentation();

        if (data.UseTls)
            mqttOptionsBuilder.WithTlsOptions(new MqttClientTlsOptions { UseTls = true });

        var clientOptions = options.Value?.Client ?? new ClientOptions();
        var mqttClient = factory.CreateMqttClient();

        var rateLimiter = new MessageRateLimiter(rateLimiterLogger, rateLimiterOptions);

        if (data.ProtocolVersion == BrokerProtocolVersion.MqttV5)
        {
            mqttOptionsBuilder
                .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V500)
                .WithMaximumPacketSize((uint)clientOptions.MaximalSizeOfMessageInBytes)
                .WithCleanStart();

            var client = new MqttV5Client(
                data.BrokerId,
                data.SessionId,
                mqttOptionsBuilder,
                clientOptions,
                rateLimiter,
                data.Subscribtions,
                mqttClient,
                messageBus,
                activityProvider,
                baseClientLogger);

            logger.LogInformation("[BrokerId: {brokerId}] [SessionId: {sessionId}] [Protocol: {protocol}] Prepared V5 client.",
                data.BrokerId,
                data.SessionId,
                data.ProtocolVersion);

            return Result<IClient>.Success(client);
        }

        if (data.ProtocolVersion == BrokerProtocolVersion.MqttV310 || data.ProtocolVersion == BrokerProtocolVersion.MqttV311)
        {
            mqttOptionsBuilder.WithCleanSession();

            var client = new MqttV3Client(
                data.BrokerId,
                data.SessionId,
                mqttOptionsBuilder,
                clientOptions,
                rateLimiter,
                data.Subscribtions,
                mqttClient,
                messageBus,
                activityProvider,
                baseClientLogger);

            logger.LogInformation("[BrokerId: {brokerId}] [SessionId: {sessionId}] [Protocol: {protocol}] Prepared V3 client.",
                data.BrokerId,
                data.SessionId,
                data.ProtocolVersion);

            return Result<IClient>.Success(client);
        }

        logger.LogError("[BrokerId: {brokerid}][SessionId: {sessionId}] Failed to create client.",
            data.BrokerId,
            data.SessionId);

        return Result<IClient>.Failure([FailedToCreateClient]);
    }
}
