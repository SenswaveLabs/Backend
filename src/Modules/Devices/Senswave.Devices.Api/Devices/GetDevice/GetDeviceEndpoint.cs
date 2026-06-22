using Senswave.Devices.Application.Devices.Features.GetDevice;

namespace Senswave.Devices.Api.Devices.GetDevice;

internal sealed class GetDeviceEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("devices/{deviceId:guid}", GetDevice)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.DevicesTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces<DeviceDto>(200)
            .Produces(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> GetDevice([FromRoute] Guid deviceId, IMediator mediator, IRequestContext context)
    {
        var query = new GetDeviceQuery
        {
            DeviceId = deviceId,
            UserId = context.UserId
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var data = result.ToResponse();
        return Results.Ok(data);
    }
}

