using MQTTnet.Client;

namespace Senswave.TestInfrastructure.TestEnvironments.Mqtt;

public static class MqttHelpers
{
    public static async Task Cleanup(IMqttClient client, CancellationToken cancellationToken = default)
    {
        await client.DisconnectAsync(cancellationToken: cancellationToken);
        client.Dispose();
        await Task.Delay(1000, cancellationToken);
    }
}
