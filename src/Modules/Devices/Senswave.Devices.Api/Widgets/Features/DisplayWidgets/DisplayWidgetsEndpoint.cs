using Senswave.Devices.Application.Widgets.Features.DisplayWidgets;

namespace Senswave.Devices.Api.Widgets.Features.DisplayWidgets;

internal sealed class DisplayWidgetsEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("devices/widgets/display", DisplayWidgets)
            .MapToApiVersion(1)
            .WithDescription("Display available widgets for device groupped with main operation.")
            .WithTags(DevicesModule.WidgetsTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces<DisplayWidgetsResponse>(200)
            .ProducesProblem(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> DisplayWidgets([FromQuery] Guid deviceId, IMediator mediator, IRequestContext context)
    {
        var query = new DisplayWidgetsQuery
        {
            UserId = context.UserId,
            DeviceId = deviceId
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToResponse();
        return Results.Ok(response);
    }
}

