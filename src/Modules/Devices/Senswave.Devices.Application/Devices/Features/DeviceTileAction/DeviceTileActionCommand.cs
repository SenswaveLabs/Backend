using Senswave.Devices.Domain.Devices.Models;
using System.Text.Json.Nodes;

namespace Senswave.Devices.Application.Devices.Features.DeviceTileAction;

public class DeviceTileActionCommand : ICommand<DisplayDeviceModel>
{
    public Guid DeviceId { get; set; }
    public Guid UserId { get; set; }

    public JsonValue Value { get; set; } = JsonValue.Create("");
}
