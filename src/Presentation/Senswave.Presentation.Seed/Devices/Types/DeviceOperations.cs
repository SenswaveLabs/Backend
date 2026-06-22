namespace Senswave.Presentation.Seed.Devices.Types;

public record DeviceOperations()
{
    public Guid BooleanOperationId { get; set; }
    public Guid NumberOperationId { get; set; }
    public Guid OptionOperationId { get; set; }
    public Guid HexOperationId { get; set; }

}