using Senswave.Integration.Shared;

namespace Senswave.Integration.DataSource.BrokerAccess;

public record BrokerAccessResponse : BaseInternalResponse
{
    public Guid OwnerId { get; set; } = Guid.Empty;
}
