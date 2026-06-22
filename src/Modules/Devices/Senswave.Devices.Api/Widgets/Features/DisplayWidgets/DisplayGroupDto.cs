namespace Senswave.Devices.Api.Widgets.Features.DisplayWidgets;

internal class DisplayGroupDto
{
    public OperationDto Operation { get; set; } = new();

    public List<WidgetDto> Widgets { get; set; } = [];
}
