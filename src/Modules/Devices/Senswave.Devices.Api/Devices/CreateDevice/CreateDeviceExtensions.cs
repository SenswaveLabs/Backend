using Senswave.Abstractions.Resulting;
using Senswave.Devices.Application.Devices.Features.CreateDevice;
using Senswave.Devices.Domain.Devices.Entities;

namespace Senswave.Devices.Api.Devices.CreateDevice;

internal static class CreateDeviceExtensions
{
    internal static DeviceCreatedResponse ToDeviceCreatedResponse(this Result<Device> result) => new()
    {
        Id = result.Data.Id,
    };

    internal static CreateDeviceCommand ToCommand(this CreateDeviceRequest dto, Guid userId) => new()
    {
        HomeId = dto.HomeId,
        RoomId = dto.RoomId,
        Icon = dto.Icon,
        Name = dto.Name,
        UserId = userId,
    };
}
