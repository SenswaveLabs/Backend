using Senswave.Devices.Api.Sharings.SetSharing;

namespace Senswave.Devices.Api.Sharings.DeleteSharing;

internal sealed class SetDeviceSharingsEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("devices/sharings", PutDeviceSharings)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.SharingTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces(204)
            .Produces(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> PutDeviceSharings([FromBody] SetSharingRequest request, IMediator mediator, IRequestContext context)
    {
        var command = request.ToCommand(context.UserId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}

