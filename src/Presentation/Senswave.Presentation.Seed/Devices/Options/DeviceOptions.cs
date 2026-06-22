namespace Senswave.Presentation.Seed.Devices.Options;

public class DeviceOptions
{
    public PicoControllerOptions PicoController { get; set; } = new();
    public bool Detector { get; set; }
    public bool EmptyDevice { get; set; }
    public bool PlantMonitor { get; set; }
}
