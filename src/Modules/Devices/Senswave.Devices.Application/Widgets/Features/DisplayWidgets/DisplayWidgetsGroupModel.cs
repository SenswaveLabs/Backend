using Senswave.Devices.Domain.Widgets.Entities;

namespace Senswave.Devices.Application.Widgets.Features.DisplayWidgets;

public class DisplayWidgetsGroupModel
{
    public OperationGroupModel Operation = new();

    public List<Widget> Widgets = [];
}
