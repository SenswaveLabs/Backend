namespace Senswave.DataSources.Application.Brokers.Brokers.Features.DeleteSubscribtion;

public class DeleteSubscribtionValidator : AbstractValidator<DeleteSubscribtionCommand>
{
    public DeleteSubscribtionValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty()
            .WithMessage("Subscription id can not be empty");

        RuleFor(x => x.BrokerId)
            .NotEmpty()
            .WithMessage("Broker id can not be empty");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User id can not be empty");
    }
}
