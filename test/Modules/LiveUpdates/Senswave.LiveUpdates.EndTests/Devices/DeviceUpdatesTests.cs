using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.Integration.DataTransfer.Devices;
using Senswave.TestInfrastructure.TestEnvironments.Base;
using System.Text.Json.Nodes;

namespace Senswave.LiveUpdates.EndTests.Devices;

[Collection(GlobalUsings.LiveUpdatesCollection)]
[Trait("Collection", "EndTest")]
public class DeviceUpdatesTests(BaseTestEnvironment factory) : BaseSignalRFeatureTest(factory)
{
    public static List<object[]> ValidHomeSharingTypes => HomePriviliges;

    [Fact]
    public async Task UserReceivesDeviceTileUpdates()
    {
        // Arrange
        var receivedDeviceId = Guid.Empty;
        var (_, connection, brokerId, homeId, deviceId) = await InitializeSignalR();
        connection.On<string, JsonObject>("Update", (actionName, data) =>
        {
            receivedDeviceId = Guid.Parse(data["deviceId"]!.ToString());
        });

        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPublishMessageBus>();

        var message = new DeviceTileActionEvent
        {
            DeviceId = deviceId
        };

        // Act
        await service.Publish(message);
        await Task.Delay(GlobalUsings.DefaultTimeout);

        // Assert
        Assert.Equal(HubConnectionState.Connected, connection.State);
        Assert.Equal(deviceId, receivedDeviceId);
    }

    [Theory]
    [MemberData(nameof(ValidHomeSharingTypes))]
    public async Task FriendReceivesDeviceTileUpdates(string sharingType)
    {
        // Arrange
        var receivedDeviceId = Guid.Empty;
        var (_, connection, brokerId, homeId, deviceId) = await InitializeSignalR();
        await PrepareHomeSharingForAdmin(homeId, sharingType);
        connection.On<string, JsonObject>("Update", (actionName, data) =>
        {
            receivedDeviceId = Guid.Parse(data["deviceId"]!.ToString());
        });

        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPublishMessageBus>();

        var message = new DeviceTileActionEvent
        {
            DeviceId = deviceId
        };

        // Act
        await service.Publish(message);
        await Task.Delay(GlobalUsings.DefaultTimeout);

        // Assert
        Assert.Equal(HubConnectionState.Connected, connection.State);
        Assert.Equal(deviceId, receivedDeviceId);
    }
}
