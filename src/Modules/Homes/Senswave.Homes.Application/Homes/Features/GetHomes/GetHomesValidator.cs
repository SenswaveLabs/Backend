namespace Senswave.Homes.Application.Homes.Features.GetHomes;

public class GetHomesValidator : AbstractValidator<GetHomesQuery>
{
    public GetHomesValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.Size)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}