namespace Senswave.Devices.Api.Devices.CreateDevice;

internal sealed class CreateDeviceEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("devices", CreateDevice)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.DevicesTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces<DeviceCreatedResponse>(201)
            .Produces(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> CreateDevice([FromBody] CreateDeviceRequest request, IMediator mediator, IRequestContext context)
    {
        var command = request.ToCommand(context.UserId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToDeviceCreatedResponse();
        return Results.Created($"/{result.Data.Id}", response);
    }
}

