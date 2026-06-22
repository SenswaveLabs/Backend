using Senswave.DataSources.Application.Brokers.Brokers.Features.DeleteBroker;

namespace Senswave.DataSources.Api.Brokers.Brokers.DeleteBroker;

internal static class DeleteBrokerExtensions
{
    internal static DeleteBrokerCommand ToDeleteBrokerCommand(this Guid brokerId, Guid userId) => new()
    {
        BrokerId = brokerId,
        UserId = userId
    };
}
