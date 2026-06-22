namespace Senswave.Homes.Application.Homes.Features.GetHome;

public class GetHomeValidator : AbstractValidator<GetHomeQuery>
{
    public GetHomeValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.HomeId)
            .NotEmpty();
    }
}