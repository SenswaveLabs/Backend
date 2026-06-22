using Senswave.Devices.Api.Sharings.GetSharings;
using Senswave.Devices.Application.ShareDevices.Features.GetDeviceSharings;

namespace Senswave.Devices.Api.Sharings.DeleteSharing;

internal sealed class GetSharingsEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("devices/sharings", DeviceSharings)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.SharingTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces<GetSharingsResponse>(200)
            .Produces(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> DeviceSharings([FromQuery] Guid deviceId, IMediator mediator, IRequestContext context)
    {
        var query = new GetDeviceSharingsQuery
        {
            DeviceId = deviceId,
            UserId = context.UserId
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToResponse();
        return Results.Ok(response);
    }
}

