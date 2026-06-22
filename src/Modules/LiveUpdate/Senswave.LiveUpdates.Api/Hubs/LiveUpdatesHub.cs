using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Senswave.Integration.Devices.Devices;
using Senswave.Integration.Homes.Home;
using Senswave.LiveUpdates.Api.Extensions;
using Senswave.LiveUpdates.Api.Services.RateLimiter;
using SignalRSwaggerGen.Attributes;
using SignalRSwaggerGen.Enums;


namespace Senswave.LiveUpdates.Api.Hubs;

[Authorize(AuthenticationSchemes = "Identity.Bearer")]
[SignalRHub(autoDiscover: AutoDiscover.MethodsAndParams, documentNames: [LiveUpdatesModule.GroupName], tag: LiveUpdatesModule.InitializationTag)]
public class LiveUpdatesHub(
    IRequestClient<HomeRequest> homeRequestCLient,
    IRequestClient<DevicesRequest> devicesRequestClient,
    IRateLimiterService rateLimiterService,
    ILogger<LiveUpdatesHub> logger)
    : Hub<ILiveUpdatesHub>
{
    [SignalRHidden]
    public override async Task OnConnectedAsync()
    {
        logger.LogInformation("[User: {user}][ConnectionId: {connnectionId}] User connected to live update hub.",
            Context.UserIdentifier,
            Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    [SignalRHidden]
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation("[User: {userId}] User disconnected to live update hub with {connnectionId}.",
            Context.UserIdentifier,
            Context.ConnectionId);

        if (exception != null)
        {
            logger.LogError("[User: {userId}] User disconnected to live update hub with {connnectionId}.",
                Context.UserIdentifier,
                Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    [SignalRMethod(summary: "Home updates initialization method.", description: "This endpoint should be invoked each time someone switches home in app to receive updates", autoDiscover: AutoDiscover.Params)]
    public async Task Initialize(string homeReferenceId)
    {
        var rateLimiting = await rateLimiterService.RateLimitingAllowsToWork(Context.ConnectionId);

        if (rateLimiting.IsFailure)
        {
            var errorMessage = rateLimiting.Errors.FirstOrDefault()?.ToString() ?? "Rate limiting error occurred.";

            logger.LogWarning("[User: {userId}][ConnectionId: {connectionId}] Rate limiting prevents to work: {error}",
                Context.UserIdentifier,
                Context.ConnectionId,
                errorMessage);

            await Clients.Caller.FailedToInitialize(errorMessage);

            return;
        }


        if (!Guid.TryParse(homeReferenceId, out var homeId))
        {
            logger.LogWarning("[User: {userId}] Argument is not valid home guid: {homeReferenceId}",
                Context.UserIdentifier,
                homeReferenceId);

            return;
        }

        try
        {
            var message = new HomeRequest
            {
                HomeId = homeId
            };

            var homeResponse = await homeRequestCLient.GetResponse<HomeResponse>(message, Context.ConnectionAborted);

            if (homeResponse.Message.IsFailure)
            {
                logger.LogWarning("[User: {userId}][Home: {homeId}] Home not found.",
                    Context.UserIdentifier,
                    homeId);

                await Clients.Caller.FailedToInitialize("No access to home.");
                return;
            }

            if (!Guid.TryParse(Context.UserIdentifier, out var userId))
            {
                logger.LogCritical("[User: {userId}] Failed to parse UserIdentifier.", Context.UserIdentifier);
                return;
            }

            if (homeResponse.Message.OwnerId != userId && !homeResponse.Message.AllowedUsers.Contains(userId))
            {
                logger.LogWarning("[User: {userId}][Home: {homeId}] User is not allowed to access home.",
                    Context.UserIdentifier,
                    homeId);

                await Clients.Caller.FailedToInitialize("No access to home.");
                return;
            }

            var devicesRequest = new DevicesRequest
            {
                UserId = userId,
                HomeId = homeId,
            };

            var devicesResponse = await devicesRequestClient.GetResponse<DevicesResponse>(devicesRequest, Context.ConnectionAborted);

            if (devicesResponse.Message.IsSuccess)
            {
                logger.LogInformation("[User: {userId}][Home: {homeId}] Adding user to devices groups.",
                    Context.UserIdentifier,
                    homeId);

                foreach (var device in devicesResponse.Message.Devices)
                {
                    var groupName = device.ToDevicesGroupName();
                    await Groups.AddToGroupAsync(Context.ConnectionId, groupName, Context.ConnectionAborted);
                }
            }

            if (homeResponse.Message.DataSourceId.HasValue)
            {
                logger.LogInformation("[User: {userId}][Home: {homeId}] Adding user to data sources groups.",
                    Context.UserIdentifier,
                    homeId);

                var dataSourceGroupName = homeResponse.Message.DataSourceId.Value.ToDataSourcesGroupName();
                await Groups.AddToGroupAsync(Context.ConnectionId, dataSourceGroupName, Context.ConnectionAborted);
            }

            await Clients.Caller.Initialized();

            logger.LogInformation("[User: {userId}][Home: {homeId}] Initialized live update.",
                Context.UserIdentifier,
                homeId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[User: {userId}][Home: {homeId}] Failed to initialize live update.",
                Context.UserIdentifier,
                homeId);
        }
    }
}