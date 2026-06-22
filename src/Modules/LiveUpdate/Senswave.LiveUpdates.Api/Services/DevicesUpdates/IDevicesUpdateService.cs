namespace Senswave.LiveUpdates.Api.Services.DevicesUpdates;

public interface IDevicesUpdateService
{
    Task UpdateDeviceTile(Guid deviceId);
    Task UpdateWidgets(Guid deviceId, List<Guid> widgets);
    Task UpdateDevicePresence(Guid deviceId);
}
