namespace Senswave.Devices.Domain.Devices.Models;

public class DeviceTileMessageModel
{
    public Guid OperationId { get; set; }

    public JsonValue Value { get; set; } = JsonValue.Create(string.Empty);
}
