namespace Senswave.DataSources.Application.Brokers.Brokers.Features.GetSubscribtions;

public class GetSubscribtionsValidator : AbstractValidator<GetSubscribtionsQuery>
{
    public GetSubscribtionsValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.BrokerId)
            .NotEmpty();

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.Size)
            .GreaterThanOrEqualTo(1);
    }
}
