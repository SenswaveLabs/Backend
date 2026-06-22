using Senswave.Integration.Shared;

namespace Senswave.Integration.Homes.HomeAccess;

public record HomeAccessResponse : BaseInternalResponse
{
    public bool HasBroker { get; set; }
}
