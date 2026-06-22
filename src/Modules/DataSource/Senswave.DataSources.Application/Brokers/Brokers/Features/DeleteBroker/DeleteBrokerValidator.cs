namespace Senswave.DataSources.Application.Brokers.Brokers.Features.DeleteBroker;

public class DeleteBrokerValidator : AbstractValidator<DeleteBrokerCommand>
{
    public DeleteBrokerValidator()
    {
        RuleFor(x => x.BrokerId)
            .NotEmpty()
            .WithMessage("Broker id can not be empty");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User Id can not be empty");
    }
}