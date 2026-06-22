namespace Senswave.DataSources.Application.Brokers.Clients.StartClient;

public class StartClientValidator : AbstractValidator<StartClientCommand>
{
    public StartClientValidator()
    {
        RuleFor(x => x.BrokerId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(256);
    }
}
