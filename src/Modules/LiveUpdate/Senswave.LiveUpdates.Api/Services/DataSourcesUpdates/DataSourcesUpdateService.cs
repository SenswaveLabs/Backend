using Microsoft.AspNetCore.SignalR;
using Senswave.LiveUpdates.Api.Diagnostics;
using Senswave.LiveUpdates.Api.Extensions;
using Senswave.LiveUpdates.Api.Hubs;
using System.Text.Json.Nodes;

namespace Senswave.LiveUpdates.Api.Services.DataSourcesUpdates;

internal sealed class DataSourcesUpdateService(
    ILiveUpdatesActivityProvider activityProvider,
    IHubContext<LiveUpdatesHub, ILiveUpdatesHub> context,
    ILogger<DataSourcesUpdateService> logger) : IDataSourcesUpdateService
{
    private const string UpdateType = "dataSourceStateUpdate";

    public Task UpdateDataSourceState(Guid dataSourceId, string state, CancellationToken cancellationToken)
    {
        using var activity = activityProvider.StartActivity("SignalR /update");

        var groupName = dataSourceId.ToDataSourcesGroupName();

        activity?.AddTag("signalr.group", groupName);
        activity?.AddTag("signalr.request.type", UpdateType);
        activity?.AddTag("signalr.request.datasource_id", dataSourceId);

        var message = new JsonObject()
        {
            ["dataSourceId"] = dataSourceId,
            ["state"] = state
        };

        logger.LogInformation("[DataSource: {dataSourceId}] Data source state update: {state}", dataSourceId, state);

        return context.Clients
            .Group(groupName)
            .Update(UpdateType, message);
    }
}
