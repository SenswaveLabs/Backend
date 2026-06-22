using Senswave.Devices.Application.Operations.Features.DisplayOperations;

namespace Senswave.Devices.Api.Operations.DisplayOperations;

internal sealed class DisplayOperationsEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("devices/operations/display", DisplayOperations)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.OperationsTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces<DisplayOperationsResponse>(200)
            .ProducesProblem(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> DisplayOperations(IMediator mediator, IRequestContext context, [FromQuery] Guid deviceId, [FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        var query = new DisplayOperationsQuery
        {
            UserId = context.UserId,
            DeviceId = deviceId,
            Page = page,
            Size = size
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToDisplayOperationResponse();
        return Results.Ok(response);
    }
}

