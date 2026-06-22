using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;

namespace Senswave.TestInfrastructure.Fixtures.Mqtt;

public class MosquittoContainerFixture : IMqttFixture
{
    private IContainer? _container;
    private IFutureDockerImage? _image;

    public string Hostname => "localhost";

    public int Port => _container!.GetMappedPublicPort(1883);

    public bool UseTls => false;

    public string Username => "admin";
    public string Password => "admin";

    public string Version => "MqttV5";

    public async Task InitializeAsync()
    {
        if (_container is not null && IsWorking())
            return;

        var dockerfileDir = Path.Combine("Fixtures", "Mqtt", "Mosquitto");

        _image = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(dockerfileDir)
            .Build();

        await _image.CreateAsync();

        var builder = new ContainerBuilder(_image)
            .WithPortBinding(1883, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(1883));

        _container = builder.Build();

        await _container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (_container is not null)
        {
            await _container.StopAsync();
            await _container.DisposeAsync();
            await _image!.DeleteAsync();
        }
    }

    public bool IsWorking() => _container!.State == TestcontainersStates.Running;
}