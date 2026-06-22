namespace Senswave.Homes.Application.Sharings.Features.GetSharings;

public class GetSharingsValidator : AbstractValidator<GetSharingsQuery>
{
    public GetSharingsValidator()
    {
        RuleFor(x => x.HomeId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}