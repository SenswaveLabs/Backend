using Senswave.Abstractions.Resulting;
using Senswave.Devices.Api.Devices.DisplayDevice;
using Senswave.Devices.Domain.Devices.Models;

namespace Senswave.Devices.Api.Devices.DisplayDevices;

internal static class DisplayDevicesExtensions
{
    internal static DisplayDevicesResponse ToResponse(this Result<List<DisplayDeviceModel>> result) => new()
    {
        Items = [.. result.Data.Select(x => x.ToDto())]
    };
}
