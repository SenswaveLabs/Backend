namespace Senswave.Devices.Api.Operations.CreateOperation;

internal sealed class CreateOperationEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("devices/operations", PostOperation)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.OperationsTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces<OperationCreatedResponse>(201)
            .ProducesProblem(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> PostOperation([FromBody] CreateOperationRequest request, IMediator mediator, IRequestContext context)
    {
        var command = request.ToCommand(context.UserId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToOperationCreatedResponse();
        return Results.Created($"/{result.Data.Id}", response);
    }
}

