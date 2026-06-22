using SignalRSwaggerGen.Attributes;
using SignalRSwaggerGen.Enums;
using System.Text.Json.Nodes;

namespace Senswave.LiveUpdates.Api.Hubs;

[SignalRHub(autoDiscover: AutoDiscover.MethodsAndParams, documentNames: [LiveUpdatesModule.GroupName], tag: LiveUpdatesModule.LiveUpdatesTag)]
public interface ILiveUpdatesHub
{
    Task Initialized();
    Task FailedToInitialize(string reason);
    Task Update(string updateType, JsonNode data);
}
