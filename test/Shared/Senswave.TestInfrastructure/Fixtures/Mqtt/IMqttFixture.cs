namespace Senswave.TestInfrastructure.Fixtures.Mqtt;

public interface IMqttFixture : IAsyncLifetime
{
    int Port { get; }
    bool UseTls { get; }
    string Hostname { get; }
    string Version { get; }

    string Password { get; }
    string Username { get; }

    bool IsWorking();
}