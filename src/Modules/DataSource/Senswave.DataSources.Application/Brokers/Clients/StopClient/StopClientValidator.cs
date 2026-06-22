namespace Senswave.DataSources.Application.Brokers.Clients.StopClient;

public class StopClientValidator : AbstractValidator<StopClientCommand>
{
    public StopClientValidator()
    {
        RuleFor(x => x.BrokerId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
