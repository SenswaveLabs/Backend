using Senswave.Devices.Domain.Operations.Entities;

namespace Senswave.Devices.Application.Operations.Features.DisplayOperations;

public class DisplayOperationsQuery : IPagedQuery<IEnumerable<Operation>>
{
    public Guid UserId { get; set; } = Guid.Empty;

    public Guid DeviceId { get; set; } = Guid.Empty;

    public int Page { get; set; }
    public int Size { get; set; }
}