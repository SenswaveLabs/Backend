namespace Senswave.DataSources.Application.Brokers.Brokers.Features.GetBrokers;

public class GetBrokersValidator : AbstractValidator<GetBrokersQuery>
{
    public GetBrokersValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.Size)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
