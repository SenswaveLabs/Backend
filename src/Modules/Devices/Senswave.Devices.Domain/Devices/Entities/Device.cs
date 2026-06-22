using Senswave.Devices.Domain.Dashboards.Entities;
using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.ShareDevices.Entites;

namespace Senswave.Devices.Domain.Devices.Entities;

public class Device : AuditableEntity
{
    /* Base Fields */
    [MaxLength(AllowedLengths.Icons.MaxLength)]
    public string Icon { get; set; } = string.Empty;

    [Required]
    [MinLength(AllowedLengths.Names.MinLength)]
    [MaxLength(AllowedLengths.Names.MaxLength)]
    public string Name { get; set; } = string.Empty;

    /* Required References */
    public Guid HomeReferenceId { get; set; }
    public HomeReference HomeReference { get; set; } = new();

    [Required]
    public DeviceTile Tile { get; set; } = new();

    /* Future Required References */
    public DevicePresence? Presence { get; set; } 

    /* Optional References */

    public Guid? RoomReferenceId { get; set; }

    public IList<DeviceSharing> DeviceSharings { get; set; } = [];

    public IList<Dashboard> Dashboards { get; set; } = [];

    public IList<Operation> Operations { get; set; } = [];

    public IList<DataSourceDataReference> DataReferences { get; set; } = [];
}
