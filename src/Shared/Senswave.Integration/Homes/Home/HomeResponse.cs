using Senswave.Integration.Shared;

namespace Senswave.Integration.Homes.Home;

public record HomeResponse : BaseInternalResponse
{
    public Guid OwnerId { get; set; }
    public Guid? DataSourceId { get; set; }

    public List<Guid> AllowedUsers { get; set; } = [];
}
