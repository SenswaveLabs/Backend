namespace Senswave.TestInfrastructure.Fixtures.Mqtt;

public class MosquittoFixture : IMqttFixture
{
    public string Hostname => "localhost";

    public int Port => 1883;

    public bool UseTls => false;

    public string Username => "admin";
    public string Password => "admin";

    public string Version => "MqttV5";

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => Task.CompletedTask;

    public bool IsWorking() => true;
}