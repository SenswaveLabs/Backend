namespace Senswave.Devices.Api.Operations.DeleteOperation;

internal sealed class DeleteOperationEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("devices/operations/{operationId:guid}", DeleteOperation)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.OperationsTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces(204)
            .ProducesProblem(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> DeleteOperation([FromRoute] Guid operationId, IMediator mediator, IRequestContext context)
    {
        var command = operationId.ToDeleteOperationCommand(context.UserId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}

