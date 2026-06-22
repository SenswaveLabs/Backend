using Refit;
using Senswave.Devices.Api.Devices.CreateDevice;
using Senswave.Devices.Api.Devices.UpdateDevice;

namespace Senswave.Presentation.Seed.Devices.Clients;

public interface IDeviceClient
{
    [Post("/v1/devices")]
    Task<DeviceCreatedResponse> CreateDevice([Authorize(scheme: "Bearer")] string token, [Body] CreateDeviceRequest home);

    [Patch("/v1/devices/{id}")]
    Task PatchDevice([Authorize(scheme: "Bearer")] string accessToken, Guid id, [Body] UpdateDeviceRequest updateDeviceRequest);
}
