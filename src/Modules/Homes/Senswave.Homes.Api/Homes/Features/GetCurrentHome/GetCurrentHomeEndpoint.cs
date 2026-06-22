using Microsoft.Extensions.Logging;
using Senswave.Homes.Application.Homes.Features.GetCurrentHome;

namespace Senswave.Homes.Api.Homes.Features.GetCurrentHome;

internal sealed class GetCurrentHomeEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("homes/current", GetCurrentHome)
            .MapToApiVersion(1)
            .WithTags(HomesModule.HomesTag)
            .WithGroupName(HomesModule.GroupName)
            .Produces<GetCurrentHomeResponse>(200)
            .Produces(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> GetCurrentHome(

    [FromServices] ILogger<GetCurrentHomeEndpoint> logger,
        IMediator mediator,
        IRequestContext context,
        [FromQuery] double? latitude = null,
        [FromQuery] double? longitude = null)
    {
        var query = new GetCurrentHomeQuery
        {
            UserId = context.UserId,
            Latitude = latitude,
            Longitude = longitude
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToHomeResponse();

        return Results.Ok(response);
    }
}

