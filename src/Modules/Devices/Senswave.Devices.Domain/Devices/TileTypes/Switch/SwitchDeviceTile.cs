using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Models;
using Senswave.Devices.Domain.Devices.TileTypes.Base;
using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Devices.Domain.Operations.Types;
using System.Text.Json;

namespace Senswave.Devices.Domain.Devices.TileTypes.Switch;

public sealed class SwitchDeviceTile(
    DeviceTile tile,
    SwitchDeviceTileConfiguration configuration,
    IOperation switchableOperation,
    ILogger<IDeviceTile> logger) : BaseDeviceTile(tile)
{
    private static readonly Error UnsupportedValue =
        Error.Failure("UnsupportedSwitchValue", "Value must be boolean or empty string for toggle.");

    private static readonly Error InvalidOperationType =
        Error.Validation("InvalidOperationType", "Switch tile requires a boolean operation.");

    public SwitchDeviceTileConfiguration Configuration => configuration;

    public override Task<Result> Validate()
    {
        if (switchableOperation.Type != OperationType.Boolean)
        {
            logger.LogError("[Tile: {tileId}] Invalid operation type {type} for switch tile.", Id, switchableOperation.Type);
            return Task.FromResult(Result.Failure(InvalidOperationType));
        }

        return Task.FromResult(Result.Success());
    }

    public override async Task<Result<DeviceTileMessageModel>> Preprocess(JsonNode value)
    {
        var kind = value.GetValueKind();

        if (kind == JsonValueKind.String && value.GetValue<string>() == string.Empty)
        {
            var currentResult = await switchableOperation.GetCurrentValue();

            JsonValue newValue;
            if (currentResult.IsSuccess)
            {
                var reversed = !currentResult.Data.Value.GetValue<bool>();
                newValue = JsonValue.Create(reversed)!;
                logger.LogWarning("[Tile: {tileId}] No value provided, toggling to {value}.", Id, reversed);
            }
            else
            {
                newValue = JsonValue.Create(true)!;
                logger.LogWarning("[Tile: {tileId}] No last value found, defaulting to true.", Id);
            }

            return Result<DeviceTileMessageModel>.Success(new DeviceTileMessageModel
            {
                OperationId = _tile.SwitchOperationId!.Value,
                Value = newValue,
            });
        }

        if (kind == JsonValueKind.True || kind == JsonValueKind.False)
        {
            logger.LogInformation("[Tile: {tileId}] Boolean value passed through.", Id);
            return Result<DeviceTileMessageModel>.Success(new DeviceTileMessageModel
            {
                OperationId = _tile.SwitchOperationId!.Value,
                Value = (JsonValue)value,
            });
        }

        logger.LogError("[Tile: {tileId}] Unsupported value kind: {kind}.", Id, kind);
        return Result<DeviceTileMessageModel>.Failure(UnsupportedValue);
    }

    public override Result<DisplayDeviceTileModel> ToDisplay()
    {
        var model = new DisplayDeviceTileModel { Type = Type };

        var latest = _tile.SwitchOperation?.Values
            .OrderByDescending(x => x.ProcessedAtUtc)
            .FirstOrDefault();

        if (latest is not null)
            model.Value = latest.Value;

        return Result<DisplayDeviceTileModel>.Success(model);
    }

    public override DeviceTile AsEntity()
    {
        _tile.Configuration = JsonSerializer.SerializeToNode(configuration)!.AsObject();
        return _tile;
    }
}
