using Microsoft.AspNetCore.SignalR;
using Senswave.LiveUpdates.Api.Diagnostics;
using Senswave.LiveUpdates.Api.Extensions;
using Senswave.LiveUpdates.Api.Hubs;
using System.Text.Json.Nodes;

namespace Senswave.LiveUpdates.Api.Services.DevicesUpdates;

public sealed class DevicesUpdateService(
    ILiveUpdatesActivityProvider activityProvider,
    IHubContext<LiveUpdatesHub, ILiveUpdatesHub> context,
    ILogger<DevicesUpdateService> logger) : IDevicesUpdateService
{
    private const string WidgetsUpdateType = "widgetsActionUpdate";

    private const string DeviceTileUpdateType = "deviceTileActionUpdate";

    private const string DevicePresenceUpdateType = "devicePresenceUpdate";

    public Task UpdateDeviceTile(Guid deviceId)
    {
        using var activity = activityProvider.StartActivity("SignalR /update");

        var groupName = deviceId.ToDevicesGroupName();

        activity?.AddTag("signalr.group", groupName);
        activity?.AddTag("signalr.request.type", DeviceTileUpdateType);
        activity?.AddTag("signalr.request.device_id", deviceId);

        var message = new JsonObject()
        {
            ["deviceId"] = deviceId
        };

        return context.Clients
            .Group(groupName)
            .Update(DeviceTileUpdateType, message);
    }

    public Task UpdateWidgets(Guid deviceId, List<Guid> widgets)
    {
        using var activity = activityProvider.StartActivity("SignalR /update");

        if (widgets.Count == 0)
        {
            logger.LogWarning("[Device: {deviceId}] No widgets to update.", deviceId);
            activity?.AddEvent(new("No widgets to update"));
            return Task.CompletedTask;
        }

        var groupName = deviceId.ToDevicesGroupName();

        activity?.AddTag("signalr.group", groupName);
        activity?.AddTag("signalr.request.type", WidgetsUpdateType);
        activity?.AddTag("signalr.request.device_id", deviceId);

        var items = new JsonArray();

        foreach (var widget in widgets)
        {
            items.Add(widget.ToString());
        }

        var message = new JsonObject
        {
            ["deviceId"] = deviceId,
            ["items"] = items
        };

        logger.LogInformation("[Device: {deviceId}] Device update for widgets: {widgetCount} widgets.", deviceId, widgets.Count);

        return context.Clients
            .Group(groupName)
            .Update(WidgetsUpdateType, message);
    }

    public Task UpdateDevicePresence(Guid deviceId)
    {
        using var activity = activityProvider.StartActivity("SignalR /update");

        var groupName = deviceId.ToDevicesGroupName();

        activity?.AddTag("signalr.group", groupName);
        activity?.AddTag("signalr.request.type", DevicePresenceUpdateType);
        activity?.AddTag("signalr.request.device_id", deviceId);

        var message = new JsonObject()
        {
            ["deviceId"] = deviceId
        };

        return context.Clients
            .Group(groupName)
            .Update(DevicePresenceUpdateType, message);
    }
}
