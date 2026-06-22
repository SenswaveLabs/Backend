using Senswave.Devices.Domain.Operations.Enums;

namespace Senswave.Devices.Application.Widgets.Features.DisplayWidgets;

public class OperationGroupModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public OperationType Type { get; set; } = OperationType.Empty;
}
