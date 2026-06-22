using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.Integration.DataTransfer.Devices;
using Senswave.TestInfrastructure.TestEnvironments.Base;
using System.Text.Json.Nodes;

namespace Senswave.LiveUpdates.EndTests.Devices;

[Collection(GlobalUsings.LiveUpdatesCollection)]
[Trait("Collection", "EndTest")]
public class WidgetUpdatesTests(BaseTestEnvironment factory) : BaseSignalRFeatureTest(factory)
{
    public static List<object[]> ValidHomeSharingTypes => HomePriviliges;

    [Fact]
    public async Task UserReceivesWidgetUpdates()
    {
        // Arrange
        var receivedDeviceId = Guid.Empty;
        var receivedWidgetId = "";
        var widgetId = Guid.NewGuid();

        var (_, connection, brokerId, homeId, deviceId) = await InitializeSignalR();
        connection.On<string, JsonObject>("Update", (actionName, data) =>
        {
            receivedDeviceId = Guid.Parse(data["deviceId"]!.ToString());
            receivedWidgetId = data["items"]!.AsArray().First()!.ToString();
        });

        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPublishMessageBus>();

        var message = new WidgetActionEvent
        {
            DeviceId = deviceId,
            WidgetIds = [widgetId]
        };

        // Act
        await service.Publish(message);
        await Task.Delay(GlobalUsings.DefaultTimeout);

        // Assert
        Assert.Equal(HubConnectionState.Connected, connection.State);
        Assert.Equal(deviceId, receivedDeviceId);
        Assert.Equal(widgetId.ToString(), receivedWidgetId);
    }

    [Fact]
    public async Task UserReceivesWidgetsUpdates()
    {
        // Arrange
        var receivedDeviceId = Guid.Empty;
        List<string> widgets = [];
        var widgetId1 = Guid.NewGuid();
        var widgetId2 = Guid.NewGuid();

        var (_, connection, brokerId, homeId, deviceId) = await InitializeSignalR();
        connection.On<string, JsonObject>("Update", (actionName, data) =>
        {
            receivedDeviceId = Guid.Parse(data["deviceId"]!.ToString());
            widgets = data["items"]!
                .AsArray()
                .ToList()
                .Select(x => x!.ToString())
                .ToList();
        });

        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPublishMessageBus>();

        var message = new WidgetActionEvent
        {
            DeviceId = deviceId,
            WidgetIds = [widgetId1, widgetId2]
        };

        // Act
        await service.Publish(message);
        await Task.Delay(GlobalUsings.DefaultTimeout);

        // Assert
        Assert.Equal(HubConnectionState.Connected, connection.State);
        Assert.Equal(deviceId, receivedDeviceId);
        Assert.Equal(2, widgets.Count);
        Assert.Contains(widgetId1.ToString(), widgets);
        Assert.Contains(widgetId2.ToString(), widgets);
    }

    [Theory]
    [MemberData(nameof(ValidHomeSharingTypes))]
    public async Task FriendReceivesWidgetUpdates(string sharingType)
    {
        // Arrange
        var receivedDeviceId = Guid.Empty;
        var receivedWidgetId = "";
        var widgetId = Guid.NewGuid();
        var (_, connection, brokerId, homeId, deviceId) = await InitializeSignalR();
        await PrepareHomeSharingForAdmin(homeId, sharingType);
        connection.On<string, JsonObject>("Update", (actionName, data) =>
        {
            receivedDeviceId = Guid.Parse(data["deviceId"]!.ToString());
            receivedWidgetId = data["items"]!.AsArray().First()!.ToString();
        });

        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPublishMessageBus>();

        var message = new WidgetActionEvent
        {
            DeviceId = deviceId,
            WidgetIds = [widgetId]
        };

        // Act
        await service.Publish(message);
        await Task.Delay(GlobalUsings.DefaultTimeout);

        // Assert
        Assert.Equal(HubConnectionState.Connected, connection.State);
        Assert.Equal(deviceId, receivedDeviceId);
    }
}
