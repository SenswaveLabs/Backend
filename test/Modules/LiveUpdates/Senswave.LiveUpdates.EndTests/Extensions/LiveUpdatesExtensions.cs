using Microsoft.AspNetCore.SignalR.Client;
using Senswave.TestInfrastructure.TestEnvironments.Common;

namespace Senswave.LiveUpdates.EndTests.Extensions;

public static class LiveUpdatesExtensions
{
    public const string DefaultUrl = "http://localhost/";

    public static string ToLiveUpdatesUrl(this HttpClient client)
        => Path.Combine(client.BaseAddress?.AbsoluteUri ?? DefaultUrl, Paths.LiveUpdatePath);

    public static async Task<HubConnection> ToSignalR(this HttpClient client, HttpMessageHandler handler, bool useAuthorization = true, bool useAdmin = false)
    {
        string? header = string.Empty;

        if (useAuthorization)
        {
            if (useAdmin)
                await Tests.AuthorizeClientWithConsent(client, "admin@gmail.com", "Admin123456!");
            else
                await Tests.AuthorizeClientAsUser(client);

            header = client.DefaultRequestHeaders.Authorization!.Parameter;
        }

        var url = client.ToLiveUpdatesUrl();

        var builder = new HubConnectionBuilder()
            .WithUrl(url, options =>
            {
                options.HttpMessageHandlerFactory = _ => handler;
                options.AccessTokenProvider = () => Task.FromResult(header);
            }).WithAutomaticReconnect();

        return builder.Build();
    }
}
