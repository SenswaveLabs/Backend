namespace Senswave.Devices.Api.Devices.DeleteDevice;

internal sealed class DeleteDeviceEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("devices/{deviceId:guid}", DeleteDevice)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.DevicesTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces(204)
            .Produces<ErrorProblemDetails>(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> DeleteDevice([FromRoute] Guid deviceId, IMediator mediator, IRequestContext context)
    {
        var command = deviceId.ToDeleteDeviceCommand(context.UserId);
        var response = await mediator.Send(command);

        if (response.IsFailure)
            return response.ToResultsDetails();

        return Results.NoContent();
    }
}

