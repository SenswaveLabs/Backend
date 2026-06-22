using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Devices.Domain.Operations.ValueObjects;
using Senswave.Devices.Domain.Widgets.Entities;

namespace Senswave.Devices.Domain.Operations.Entities;

public class Operation : AuditableEntity
{
    public Guid DeviceId { get; set; }

    public Device? Device { get; set; }

    public Guid? DataReferenceId { get; set; }

    public DataSourceDataReference? DataReference { get; set; }

    [Required]
    [MinLength(AllowedLengths.Names.MinLength)]
    [MaxLength(AllowedLengths.Names.MaxLength)]
    public string Name { get; set; } = string.Empty;

    public OperationType Type { get; set; }

    public JsonObject Configuration { get; set; } = [];

    public List<OperationValue> Values { get; set; } = [];

    public List<Widget> Widgets { get; set; } = [];
}
