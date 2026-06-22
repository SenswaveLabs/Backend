using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Widgets.Enums;

namespace Senswave.Devices.Domain.Widgets.Entities;

public class Widget : AuditableEntity
{
    public Guid OperationId { get; set; }

    public Operation? Operation { get; set; }

    [Required]
    [MinLength(AllowedLengths.Names.MinLength)]
    [MaxLength(AllowedLengths.Names.MaxLength)]
    public string Name { get; set; } = string.Empty;

    public bool Enabled { get; set; } = true;

    public WidgetType Type { get; set; }

    public JsonObject Configuration { get; set; } = [];
}
