
namespace Senswave.Homes.Application.Homes.Features.GetCurrentHome;

public class GetCurrentHomeValidator : AbstractValidator<GetCurrentHomeQuery>
{
    public GetCurrentHomeValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180);
    }
}