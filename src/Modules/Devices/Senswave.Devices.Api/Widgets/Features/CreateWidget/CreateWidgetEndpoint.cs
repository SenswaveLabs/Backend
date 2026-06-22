namespace Senswave.Devices.Api.Widgets.Features.CreateWidget;

internal sealed class CreateWidgetEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("devices/widgets", CreateWidget)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.WidgetsTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces<WidgetCreatedResponse>(201)
            .ProducesProblem(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(409);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> CreateWidget([FromBody] CreateWidgetRequest dto, IMediator mediator, IRequestContext context)
    {
        var request = dto.ToCommand(context.UserId);

        if (request == null)
            return Results.BadRequest();

        var result = await mediator.Send(request);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToWidgetCreatedResponse();
        return Results.Created($"/{result.Data.Id}", response);
    }
}

