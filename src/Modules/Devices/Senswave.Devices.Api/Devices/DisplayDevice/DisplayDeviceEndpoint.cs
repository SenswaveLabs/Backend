using Senswave.Devices.Application.Devices.Features.DisplayDevice;

namespace Senswave.Devices.Api.Devices.DisplayDevice;

internal sealed class DisplayDeviceEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("devices/{deviceId:guid}/display", DisplayDevice)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.DevicesTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces<DisplayDeviceDto>(200)
            .Produces(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> DisplayDevice([FromRoute] Guid deviceId, IMediator mediator, IRequestContext context)
    {
        var query = new DisplayDeviceQuery
        {
            UserId = context.UserId,
            DeviceId = deviceId
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToDto();
        return Results.Ok(response);
    }
}

