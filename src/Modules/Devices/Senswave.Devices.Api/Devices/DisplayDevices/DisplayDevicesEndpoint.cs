using Senswave.Devices.Application.Devices.Features.DisplayDevices;

namespace Senswave.Devices.Api.Devices.DisplayDevices;

internal sealed class DisplayDevicesEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("devices/display", DisplayDevices)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.DevicesTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces<DisplayDevicesResponse>(200)
            .Produces(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> DisplayDevices([FromQuery] Guid homeId, [FromQuery] int? page, [FromQuery] int? size, IMediator mediator, IRequestContext context)
    {
        var query = new DisplayDevicesQuery
        {
            UserId = context.UserId,
            HomeReferenceId = homeId,
            Page = page ?? 1,
            Size = size ?? 10
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToResponse();
        return Results.Ok(response);
    }
}

