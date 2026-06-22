
namespace Senswave.TestInfrastructure.Fixtures.Mqtt;

public class HiveMqCloudContainerFixture : IMqttFixture
{
    public string Hostname => "da9c85bf397c4910b03ad4656cf8cd67.s1.eu.hivemq.cloud";
    public int Port => 8883;
    public bool UseTls => true;
    public string Version => "MqttV5";

    public string Username => "Seed1";
    public string Password => "Seed123!";

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public bool IsWorking() => true;
}
