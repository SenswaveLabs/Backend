using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Models;
using Senswave.Devices.Domain.Devices.TileTypes.Base;
using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Devices.Domain.Operations.Extensions;
using Senswave.Devices.Domain.Operations.Types;
using System.Text.Json;

namespace Senswave.Devices.Domain.Devices.TileTypes.Display;

public sealed class DisplayDeviceTile(
    DeviceTile tile,
    DisplayDeviceTileConfiguration configuration,
    IOperation displayableOperation,
    ILogger<IDeviceTile> logger) : BaseDeviceTile(tile)
{
    private static readonly Error NotSupported =
        Error.Failure("DisplayTileActionNotSupported", "Display tile does not support sending values.");

    private static readonly Error InvalidOperationType =
        Error.Validation("InvalidOperationType", "Display tile requires a numeric operation (Number or Integer).");

    public DisplayDeviceTileConfiguration Configuration => configuration;

    public override async Task<Result> Validate()
    {
        if (displayableOperation.Type != OperationType.Number && displayableOperation.Type != OperationType.Integer)
        {
            logger.LogError("[Tile: {tileId}] Invalid operation type {type} for display tile.", Id, displayableOperation.Type);
            return Result.Failure(InvalidOperationType);
        }

        var validator = new DisplayDeviceTileValidator();
        var validationResult = await validator.ValidateAsync(this);

        if (!validationResult.IsValid)
            return validationResult.ToResult();

        return Result.Success();
    }

    public override Task<Result<DeviceTileMessageModel>> Preprocess(JsonNode value) =>
        Task.FromResult(Result<DeviceTileMessageModel>.Failure(NotSupported));

    public override Result<DisplayDeviceTileModel> ToDisplay()
    {
        var model = new DisplayDeviceTileModel { Type = Type, Configuration = _tile.Configuration };

        var latest = _tile.DisplayableOperation?.Values
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
