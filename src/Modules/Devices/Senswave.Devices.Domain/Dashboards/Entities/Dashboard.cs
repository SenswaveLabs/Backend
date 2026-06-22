using Senswave.Devices.Domain.Dashboards.Enums;
using Senswave.Devices.Domain.Devices.Entities;
namespace Senswave.Devices.Domain.Dashboards.Entities;

public class Dashboard : AuditableEntity
{
    public Guid DeviceId { get; set; }

    public Device? Device { get; set; }

    [MaxLength(AllowedLengths.Icons.MaxLength)]
    public string Icon { get; set; } = string.Empty;

    [Required]
    [MinLength(AllowedLengths.Names.MinLength)]
    [MaxLength(AllowedLengths.Names.MaxLength)]
    public string Name { get; set; } = string.Empty;

    public DashboardType Type { get; set; } = DashboardType.Grid;

    public JsonObject Configuration { get; set; } = [];
}
