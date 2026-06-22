namespace Senswave.Homes.Api.Sharings.CreateSharing;

internal sealed class CreateSharingEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("homes/sharings", CreateSharing)
            .MapToApiVersion(1)
            .WithTags(HomesModule.SharingTag)
            .WithGroupName(HomesModule.GroupName)
            .Produces<HomeSharingCreatedResponse>(201)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(409);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> CreateSharing([FromBody] CreateSharingRequest request, IMediator mediator, IRequestContext context)
    {
        var command = request.ToCommand(context.UserId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToResponse();
        return Results.Created($"/{result.Data.Id}", response);
    }
}

