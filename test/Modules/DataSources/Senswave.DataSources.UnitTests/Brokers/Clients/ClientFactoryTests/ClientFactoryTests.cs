using Senswave.DataSources.BrokerConnection.Clients;
using Senswave.DataSources.BrokerConnection.Clients.MqttV3;
using Senswave.DataSources.Domain.Brokers.Brokers.Enums;

namespace Senswave.DataSources.UnitTests.Brokers.Clients.ClientFactoryTests;

[Trait("Collection", "UnitTests")]
public class ClientFactoryTests : IClassFixture<ClientFactoryFixture>
{
    private readonly ClientFactoryFixture _fixture;

    public ClientFactoryTests(ClientFactoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private void CreatesMqttV5Client()
    {
        //Arrange
        var clientModel = _fixture.BaseClientModel(BrokerProtocolVersion.MqttV5);

        //Act
        var clientResult = _fixture.ClientFactory.Create(clientModel);

        //Assert
        Assert.NotNull(clientResult);
        Assert.True(clientResult.IsSuccess);
        Assert.IsType<MqttV5Client>(clientResult.Data);
    }

    [Fact]
    private void CreatingMqttV310Client()
    {
        //Arrange
        var clientModel = _fixture.BaseClientModel(BrokerProtocolVersion.MqttV310);

        //Act
        var clientResult = _fixture.ClientFactory.Create(clientModel);

        //Assert
        Assert.NotNull(clientResult);
        Assert.True(clientResult.IsSuccess);
        Assert.IsType<MqttV3Client>(clientResult.Data);
    }

    [Fact]
    private void CreatingMqttV311Client()
    {
        //Arrange
        var clientModel = _fixture.BaseClientModel(BrokerProtocolVersion.MqttV311);

        //Act
        var clientResult = _fixture.ClientFactory.Create(clientModel);

        //Assert
        Assert.NotNull(clientResult);
        Assert.True(clientResult.IsSuccess);
        Assert.IsType<MqttV3Client>(clientResult.Data);
    }
}
