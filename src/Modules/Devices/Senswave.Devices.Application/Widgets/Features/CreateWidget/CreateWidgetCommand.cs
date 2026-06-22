using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Enums;
using System.Text.Json.Nodes;

namespace Senswave.Devices.Application.Widgets.Features.CreateWidget;

public class CreateWidgetCommand : ICommand<Widget>
{
    public Guid UserId { get; set; } = Guid.Empty;

    public Guid OperationId { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;

    public WidgetType Type { get; set; } = WidgetType.Invalid;

    public JsonObject Configuration { get; set; } = [];
}