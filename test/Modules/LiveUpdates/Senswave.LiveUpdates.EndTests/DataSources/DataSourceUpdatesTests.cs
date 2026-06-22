using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.Integration.DataTransfer.DataSourceState;
using Senswave.TestInfrastructure.TestEnvironments.Base;
using System.Text.Json.Nodes;

namespace Senswave.LiveUpdates.EndTests.DataSources;

[Collection(GlobalUsings.LiveUpdatesCollection)]
[Trait("Collection", "EndTest")]
public class DataSourceUpdatesTests(BaseTestEnvironment factory) : BaseSignalRFeatureTest(factory)
{
    public static List<object[]> ValidHomeSharingTypes => HomePriviliges;

    [Fact]
    public async Task UserReceivesDataSourceStateUpdate()
    {
        // Arrange
        var resultData = new JsonObject();
        var (_, connection, brokerId, homeId, deviceId) = await InitializeSignalR();
        connection.On<string, JsonObject>("Update", (actionName, data) =>
        {
            resultData = data;
        });

        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPublishMessageBus>();

        var message = new DataSourceStateEvent
        {
            DataSourceId = brokerId,
            State = "Stopped"
        };

        // Act
        await service.Publish(message);
        await Task.Delay(GlobalUsings.DefaultTimeout);

        // Assert
        Assert.Equal(HubConnectionState.Connected, connection.State);
        Assert.Equal(brokerId.ToString(), resultData["dataSourceId"]?.ToString());
        Assert.Equal("Stopped", resultData["state"]?.ToString());
    }

    [Theory]
    [MemberData(nameof(ValidHomeSharingTypes))]
    public async Task FriendReceivesDataSourceStateUpdates(string sharingType)
    {
        // Arrange
        var resultData = new JsonObject();
        var (_, connection, brokerId, homeId, deviceId) = await InitializeSignalR();
        await PrepareHomeSharingForAdmin(homeId, sharingType);
        connection.On<string, JsonObject>("Update", (actionName, data) =>
        {
            resultData = data;
        });

        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPublishMessageBus>();

        var message = new DataSourceStateEvent
        {
            DataSourceId = brokerId,
            State = "Working"
        };

        // Act
        await service.Publish(message);
        await Task.Delay(GlobalUsings.DefaultTimeout);

        // Assert
        Assert.Equal(HubConnectionState.Connected, connection.State);
        Assert.Equal(brokerId.ToString(), resultData["dataSourceId"]?.ToString());
        Assert.Equal("Working", resultData["state"]?.ToString());
    }
}
