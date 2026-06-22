using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.DataSources.Infrastructure;
using Senswave.Devices.Infrastructure;
using Senswave.TestInfrastructure.TestEnvironments.Mqtt;

namespace Senswave.Users.EndTests.Mqtt.Users;

[Collection("Mqtt E2E Tests Collection")]
[Trait("Collection", "MqttEndTest")]
public class DeleteAccountTests(MqttTestEnvironment factory) : MqttFeatureTest(factory)
{
    [Fact]
    public async Task DataSourcesDataRemoved()
    {
        // Arrange
        var (client, _, _) = await CreateClientWithConsent();
        var scope = Factory.Server.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataSourcesContext>();
        var devicesContext = scope.ServiceProvider.GetRequiredService<DevicesContext>();
        var clients = Services.GetRequiredService<IClientService>();

        // Act
        var dataSourceId = await PostMqttBroker(client);
        var homeId = await client.PostHome();
        await client.PutDataSourceForHome(dataSourceId, homeId);
        var deviceId = await client.PostDevice(homeId, Guid.Empty);
        var operationId = await client.PostBooleanOperation(deviceId);

        var operation = await devicesContext.Operations
            .Include(x => x.DataReference)
            .FirstAsync(x => x.Id == operationId);

        var subscribtion = await context.Subscribtions
            .FirstAsync(x => x.Id == operation.DataReference!.DataSourceDataReferenceId);

        await StartBrokerClient(client, dataSourceId);
        await Task.Delay(5000);

        var connectionExists = clients.GetClient(dataSourceId).IsSuccess;

        var sessionExistsBefore = await context.Sessions
            .AnyAsync(x => x.BrokerId == dataSourceId);

        var logExistsBefore = await context.Logs
            .AnyAsync(x => x.Session.BrokerId == dataSourceId);

        var response = await client.DeleteAsync(Base.Users.DeleteAccountTests.Path);

        var connectionExistsAfter = clients.GetClient(dataSourceId).IsSuccess;

        var doesNotExistAfter = await context.Brokers
            .AnyAsync(x => x.Id == dataSourceId);

        var subscribtionDoesNotExistAfter = await context.Subscribtions
            .AnyAsync(x => x.Id == subscribtion.Id);

        var sessionDoesNotExistAfter = await context.Sessions
            .AnyAsync(x => x.BrokerId == dataSourceId);

        var sessionLogsDoNotExistAfter = await context.Logs
            .AnyAsync(x => x.Session.BrokerId == dataSourceId);

        // Assert
        Assert.True(connectionExists);
        Assert.True(sessionExistsBefore);
        Assert.True(logExistsBefore);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.False(connectionExistsAfter);
        Assert.False(doesNotExistAfter);
        Assert.False(subscribtionDoesNotExistAfter);
        Assert.False(sessionDoesNotExistAfter);
        Assert.False(sessionLogsDoNotExistAfter);
    }
}
