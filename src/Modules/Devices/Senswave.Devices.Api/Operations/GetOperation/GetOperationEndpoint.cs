using Senswave.Devices.Application.Operations.Features.GetOperation;

namespace Senswave.Devices.Api.Operations.GetOperation;

internal sealed class GetOperationEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("devices/operations/{operationId:guid}", GetOperation)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.OperationsTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces<GetOperationResponse>(200)
            .ProducesProblem(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> GetOperation([FromRoute] Guid operationId, IMediator mediator, IRequestContext context)
    {
        var query = new GetOperationQuery
        {
            OperationId = operationId,
            UserId = context.UserId
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToOperationResponse();
        return Results.Ok(response);
    }
}

