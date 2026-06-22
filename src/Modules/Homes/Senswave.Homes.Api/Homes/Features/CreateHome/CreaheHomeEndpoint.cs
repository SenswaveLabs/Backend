namespace Senswave.Homes.Api.Homes.Features.CreateHome;

internal sealed class CreateHomeEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("homes", CreateHome)
            .MapToApiVersion(1)
            .WithTags(HomesModule.HomesTag)
            .WithGroupName(HomesModule.GroupName)
            .Produces(201)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(409);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    [ApiExplorerSettings(GroupName = "home")]
    private async Task<IResult> CreateHome([FromBody] CreateHomeRequest request, IMediator mediator, IRequestContext context)
    {
        var command = request.ToCommand(context.UserId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToCreatedResponse();
        return Results.Created($"/{result.Data.Id}", response);
    }
}

