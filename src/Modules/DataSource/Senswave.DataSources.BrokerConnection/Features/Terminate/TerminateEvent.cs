using Senswave.Abstractions.Events;
using Senswave.DataSources.Domain.Brokers.Clients.Entities;

namespace Senswave.DataSources.BrokerConnection.Features.Terminate;

public class TerminateEvent : IModuleEvent
{
    public IClient? Client { get; set; }
}
