using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Senswave.LiveUpdates.Api.Hubs;

namespace Senswave.LiveUpdates.Api;

public static class LiveUpdatesExtensions
{
    public static IEndpointRouteBuilder UseLiveUpdates(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapHub<LiveUpdatesHub>("signalr/liveupdates/live")
            .WithGroupName(LiveUpdatesModule.GroupName)
            .WithTags(LiveUpdatesModule.LiveUpdatesTag);

        return endpoints;
    }
}
