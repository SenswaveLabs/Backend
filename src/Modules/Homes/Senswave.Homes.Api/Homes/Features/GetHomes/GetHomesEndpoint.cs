using Senswave.Homes.Application.Homes.Features.GetHomes;

namespace Senswave.Homes.Api.Homes.Features.GetHomes;

internal sealed class GetHomesEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("homes", GetHomes)
            .MapToApiVersion(1)
            .WithTags(HomesModule.HomesTag)
            .WithGroupName(HomesModule.GroupName)
            .Produces<GetHomesResponse>(200)
            .Produces(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> GetHomes(IMediator mediator, IRequestContext context, [FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        var query = new GetHomesQuery
        {
            Page = page,
            Size = size,
            UserId = context.UserId
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var data = result.ToResponse(context.UserId);
        return Results.Ok(data);
    }
}
