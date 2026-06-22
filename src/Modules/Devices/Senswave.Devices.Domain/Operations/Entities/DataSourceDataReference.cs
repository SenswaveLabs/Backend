using Senswave.Devices.Domain.Devices.Entities;

namespace Senswave.Devices.Domain.Operations.Entities;

public class DataSourceDataReference : Entity
{
    public Guid DeviceId { get; set; }

    public Device? Device { get; set; }

    public List<Operation> Operations { get; set; } = [];

    public Guid DataSourceDataReferenceId { get; set; }
}
