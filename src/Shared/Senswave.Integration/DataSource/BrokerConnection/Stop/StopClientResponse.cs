using Senswave.Integration.Shared;

namespace Senswave.Integration.DataSource.BrokerConnection.Stop;

public record StopClientResponse : BaseInternalResponse
{
    public Guid SessionId { get; set; }
}
