using Senswave.TestInfrastructure.TestEnvironments.Common;
using Senswave.TestInfrastructure.TestSetup.Models;
using Senswave.TestInfrastructure.TestSetup.Models.Common;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Senswave.TestInfrastructure.Extensions;

public static class DeviceHttpClientExtensions
{
    public static async Task<Guid> PostDevice(this HttpClient client, Guid homeId, Guid roomId)
    {
        var request = RequestFactory.CreatePostDevice(
            homeId: homeId,
            icon: "default-icon",
            name: Guid.NewGuid().ToString());

        if (roomId != Guid.Empty)
            request["roomId"] = roomId;

        var response = await client.PostAsync(Paths.DevicesPath, request.Serialize());

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);

        return postResponse!.Id;
    }

    public static async Task SetSwitchTile(this HttpClient client, Guid deviceId, Guid operationId)
    {
        var request = RequestFactory.CreateAssignOperationToDevice(
            operationId: operationId.ToString(),
            type: "Switch");

        var response = await client.PatchAsync($"{Paths.DevicesPath}/{deviceId}", request.Serialize());
        response.EnsureSuccessStatusCode();
    }

    public static async Task<Guid> PostDashboard(this HttpClient client, Guid deviceId)
    {
        var request = RequestFactory.CreatePostDashboard(
            deviceId: deviceId,
            name: Guid.NewGuid().ToString(),
            configuration: new JsonObject
            {
                ["rows"] = 6,
                ["columns"] = 4
            },
            icon: "default-icon").Serialize();

        var response = await client.PostAsync(Paths.DashboardsPath, request);
        var responseContent = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        return postResponse!.Id;
    }

    public static async Task SetWidgetOnDashboard(this HttpClient client, Guid dashboardId, Guid widgetId)
    {
        var request = new JsonObject()
        {
            ["widgetId"] = widgetId,
            ["row"] = 0,
            ["rowSpan"] = 1,
            ["column"] = 0,
            ["columnSpan"] = 2
        };

        var response = await client.PutAsync($"{Paths.DashboardsPath}/{dashboardId}/widgets", request.Serialize());
        response.EnsureSuccessStatusCode();
    }

    public static async Task<Guid> PostBooleanOperation(this HttpClient client, Guid deviceId)
    {
        var request = RequestFactory.CreatePostOperation(
            deviceId: deviceId,
            name: Guid.NewGuid().ToString()[..29],
            type: "Boolean",
            configuration: new JsonObject
            {
                ["jsonNames"] = new JsonArray("value"),
                ["isJson"] = true
            },
            topic: Guid.NewGuid().ToString().Replace("-", "/"));

        var response = await client.PostAsync(Paths.OperationsPath, request.Serialize());
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        return postResponse!.Id;
    }

    public static async Task<Guid> PostHexColorOperation(this HttpClient client, Guid deviceId, JsonObject? configuration = null)
    {
        configuration ??= new JsonObject
        {
            ["jsonNames"] = new JsonArray("newValue"),
            ["isJson"] = true
        };

        var request = RequestFactory.CreatePostOperation(
            deviceId: deviceId,
            name: Guid.NewGuid().ToString()[..29],
            type: "HexColor",
            configuration: configuration,
            topic: Guid.NewGuid().ToString()).Serialize();

        var response = await client.PostAsync(Paths.OperationsPath, request);

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);

        return postResponse!.Id;
    }

    public static Task<Guid> PostBooleanButtonWidget(this HttpClient client, Guid operationId) => client.CreateButtonWidget(
        operationId: operationId,
        config: new JsonObject
        {
            ["value"] = true,
        });

    public static async Task<Guid> CreateButtonWidget(this HttpClient client, Guid operationId, JsonObject config)
    {
        var request = RequestFactory.CreatePostWidget(
            operationId: operationId,
            name: "Widget" + Guid.NewGuid(),
            type: "Button",
            config: config);

        var response = await client.PostAsync(Paths.WidgetsPath, request.Serialize());
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        return postResponse!.Id;
    }

    public static async Task<Guid> PostColorWidget(this HttpClient client, Guid operationId, JsonObject? config = null)
    {
        config ??= [];

        var request = RequestFactory.CreatePostWidget(
            operationId: operationId,
            name: "Widget" + Guid.NewGuid(),
            type: "Color",
            config: config);

        var response = await client.PostAsync(Paths.WidgetsPath, request.Serialize());
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        return postResponse!.Id;
    }

    public static async Task PutDeviceSharing(this HttpClient client, Guid deviceId, string email, string sharingType = "Manage")
    {
        var request = RequestFactory.CreatePostDeviceSharing(
          deviceId: deviceId,
          friendEmail: email,
          sharingType: sharingType);

        var response = await client.PutAsync($"{Paths.DevicesPath}/sharings", request.Serialize());
        response.EnsureSuccessStatusCode();
    }
}
