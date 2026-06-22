namespace Senswave.Devices.Api.Devices.UpdateDevice;

internal sealed class UpdateDeviceEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPatch("devices/{deviceId:guid}", UpdateDevice)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.DevicesTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces(204)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(409);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> UpdateDevice([FromRoute] Guid deviceId, [FromBody] UpdateDeviceRequest request, IMediator mediator, IRequestContext context)
    {
        var command = request.ToCommand(context.UserId, deviceId);

        var response = await mediator.Send(command);

        if (response.IsFailure)
            return response.ToResultsDetails();

        return Results.NoContent();
    }
}

