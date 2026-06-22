using MQTTnet;
using MQTTnet.Adapter;
using MQTTnet.Client;
using MQTTnet.Exceptions;
using MQTTnet.Formatter;
using Senswave.DataSources.Domain.Brokers.Brokers.Enums;
using Senswave.DataSources.Domain.Brokers.Brokers.Services;

namespace Senswave.DataSources.Application.Brokers.Brokers.Services;

internal sealed class BrokerService(
    MqttFactory factory,
    ILogger<BrokerService> logger) : IBrokerService
{
    #region Error

    private readonly static Error UnknowErrorWhenTestingConnection = Error.ServerError("UnknowErrorWhenTestingConnection", "An unknown error occurred while testing the broker connection.");
    private readonly static Error FailedToConnectWithServer = Error.Failure("FailedToConnectWithServer", "Failed to connect with the broker server.");
    private readonly static Error YourServerIsNotReachable = Error.Failure("YourServerIsNotReachable", "The server is not reachable. Please check host and port settings.");
    private readonly static Error UnexpectedDisconnection = Error.Failure("UnexpectedDisconnection", "Client disconnected unexpectedly.");
    private readonly static Error OperationTimedout = Error.Failure("OperationTimedout", "Please try again.");

    #endregion

    public async Task<Result> TestConnection(string url,
        int port,
        string clientName,
        bool useTls,
        BrokerProtocolVersion brokerProtocolVersion,
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        using var mqttClient = factory.CreateMqttClient();

        var disconnectOptions = new MqttClientDisconnectOptionsBuilder()
            .WithReason(MqttClientDisconnectOptionsReason.NormalDisconnection)
            .Build();

        try
        {
            var protocolVersion = ToMqttNetProtocolVersion(brokerProtocolVersion);

            var mqttOptionsBuilder = new MqttClientOptionsBuilder()
                .WithClientId(clientName)
                .WithTcpServer(url, port)
                .WithCredentials(username, password)
                .WithProtocolVersion(protocolVersion);

            if (useTls)
                mqttOptionsBuilder.WithTlsOptions(new MqttClientTlsOptions { UseTls = true });

            var mqttOptions = mqttOptionsBuilder.Build();

            var result = await mqttClient.ConnectAsync(mqttOptions, cancellationToken);

            if (result.ResultCode == MqttClientConnectResultCode.Success)
            {
                logger.LogInformation("[Broker url: {brokerUrl}][Port: {port}] Broker connection test successful.", url, port);
                await mqttClient.DisconnectAsync(disconnectOptions, cancellationToken);
                return Result.Success();
            }

            logger.LogWarning("[Broker url: {brokerUrl}][Port: {port}] Broker connection test failed with result code: {resultCode}", url, port, result.ResultCode);
            return Result.Failure(FailedToConnectWithServer);
        }
        catch (MqttConnectingFailedException ex)
        {
            logger.LogInformation(ex, "[Broker url: {brokerUrl}][Port: {port}] Broker connection test failed due to fail with connecting to broker", url, port);
            return Result.Failure(FailedToConnectWithServer);
        }
        catch (MqttClientDisconnectedException ex)
        {
            logger.LogInformation(ex, "[Broker url: {brokerUrl}][Port: {port}] Broker connection test failed due to unexpected disconnection.", url, port);
            return Result.Failure(UnexpectedDisconnection);
        }
        catch (MqttCommunicationException ex)
        {
            logger.LogInformation(ex, "[Broker url: {brokerUrl}][Port: {port}] Broker connection test failed to find broker.", url, port);
            return Result.Failure(YourServerIsNotReachable);
        }
        catch (OperationCanceledException ex)
        {
            logger.LogInformation(ex, "[Broker url: {brokerUrl}][Port: {port}] Btoker connection test timed out.", url, port);
            return Result.Failure(OperationTimedout);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Broker url: {brokerUrl}][Port: {port}] Error testing connection to broker.", url, port);
            return Result.Failure(UnknowErrorWhenTestingConnection);
        }
        finally
        {
            mqttClient.Dispose();
        }
    }

    private static MqttProtocolVersion ToMqttNetProtocolVersion(BrokerProtocolVersion version) => version switch
    {
        BrokerProtocolVersion.MqttV310 => MqttProtocolVersion.V310,
        BrokerProtocolVersion.MqttV311 => MqttProtocolVersion.V311,
        BrokerProtocolVersion.MqttV5 => MqttProtocolVersion.V500,
        _ => throw new ArgumentOutOfRangeException(nameof(version), version, null)
    };
}
