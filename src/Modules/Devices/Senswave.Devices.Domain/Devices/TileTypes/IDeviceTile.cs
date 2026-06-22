using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Models;

namespace Senswave.Devices.Domain.Devices.TileTypes;

public interface IDeviceTile
{
    Task<Result> Validate();

    Task<Result<DeviceTileMessageModel>> Preprocess(JsonNode value);

    Result<DisplayDeviceTileModel> ToDisplay();

    DeviceTile AsEntity();
}
