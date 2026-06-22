using Senswave.Devices.Api.Devices.DisplayDevice;

namespace Senswave.Devices.Api.Devices.DeviceTileAction;

internal sealed class DeviceTileActionEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("devices/{deviceId:guid}/tile/action", DeviceTileAction)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.DevicesTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces<DisplayDeviceDto>(200)
            .Produces(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> DeviceTileAction([FromBody] DeviceTileActionRequest request, [FromRoute] Guid deviceId, IMediator mediator, IRequestContext context)
    {
        var command = request.ToCommand(context.UserId, deviceId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToDto();
        return Results.Ok(response);
    }
}
