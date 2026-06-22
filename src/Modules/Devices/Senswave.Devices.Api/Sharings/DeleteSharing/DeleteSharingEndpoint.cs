namespace Senswave.Devices.Api.Sharings.DeleteSharing;

internal sealed class DeleteDeviceSharingEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("devices/sharings/{deviceSharingId:guid}", DeleteDeviceSharing)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.SharingTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces(204)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> DeleteDeviceSharing([FromRoute] Guid deviceSharingId, IMediator mediator, IRequestContext context)
    {
        var command = deviceSharingId.ToDeleteDeviceSharingsCommand(context.UserId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}

