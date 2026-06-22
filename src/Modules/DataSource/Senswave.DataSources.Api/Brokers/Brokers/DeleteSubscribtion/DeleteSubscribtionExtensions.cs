using Senswave.DataSources.Application.Brokers.Brokers.Features.DeleteSubscribtion;

namespace Senswave.DataSources.Api.Brokers.Brokers.DeleteSubscribtion;

internal static class DeleteSubscribtionExtensions
{
    internal static DeleteSubscribtionCommand ToDeleteSubscribtionCommand(
        this Guid subscriptionId, Guid brokerId, Guid userId) => new()
        {
            SubscriptionId = subscriptionId,
            BrokerId = brokerId,
            UserId = userId
        };
}
