using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Models;
using Senswave.Devices.Domain.Devices.TileTypes.Base;

namespace Senswave.Devices.Domain.Devices.TileTypes.Default;

internal sealed class DefaultDeviceTile(DeviceTile tile) : BaseDeviceTile(tile)
{
    private static readonly Error NotSupported =
        Error.Failure("DefaultTileNotSupported", "Default tile does not support actions.");

    public override Task<Result> Validate() =>
        Task.FromResult(Result.Success());

    public override Task<Result<DeviceTileMessageModel>> Preprocess(JsonNode value) =>
        Task.FromResult(Result<DeviceTileMessageModel>.Failure(NotSupported));

    public override Result<DisplayDeviceTileModel> ToDisplay() =>
        Result<DisplayDeviceTileModel>.Success(new DisplayDeviceTileModel { Type = Type });

    public override DeviceTile AsEntity() => _tile;
}
