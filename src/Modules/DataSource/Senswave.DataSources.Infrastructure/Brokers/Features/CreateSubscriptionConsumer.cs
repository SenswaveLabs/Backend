using MediatR;
using Senswave.DataSources.Application.Brokers.Brokers.Features.CreateSubscribtion;
using Senswave.Integration.DataSource.CreateSubscription;
using Senswave.Integration.Shared;

namespace Senswave.DataSources.Infrastructure.Brokers.Features;

public class CreateSubscriptionConsumer(
    IMediator mediator,
    ILogger<CreateSubscriptionConsumer> logger) : IConsumer<CreateSubscriptionRequest>
{
    private static CreateSubscriptionResponse Failure(Error error) => new()
    {
        StatusCode = InternalRequestStatus.Failure,
        Error = error
    };

    private static CreateSubscriptionResponse Success(Guid subscribtionId) => new()
    {
        StatusCode = InternalRequestStatus.Success,
        SubscriptionId = subscribtionId
    };

    public async Task Consume(ConsumeContext<CreateSubscriptionRequest> context)
    {
        var command = new CreateSubscribtionCommand
        {
            BrokerId = context.Message.BrokerId,
            FailWhenTopicAlreadyExists = false,
            Topic = context.Message.Topic
        };

        var result = await mediator.Send(command);

        if (result.IsSuccess)
        {
            logger.LogInformation("[Broker: {brokerId}] Consumer created subscribtion.",
                context.Message.BrokerId);

            await context.RespondAsync(Success(result.Data));
            return;
        }

        logger.LogError("[Broker: {brokerId}] Consumer failed to create subscribtion.",
            context.Message.BrokerId);

        await context.RespondAsync(Failure(result.Errors.First()));
    }
}