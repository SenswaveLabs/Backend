namespace Senswave.DataSources.Application.Brokers.Sessions.GetSessions;

public class GetSessionsValidator : AbstractValidator<GetSessionsQuery>
{
    public GetSessionsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.Size)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
