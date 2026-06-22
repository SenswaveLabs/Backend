using Senswave.Devices.Domain.Operations.Entities;
using System.Text.Json;

namespace Senswave.Devices.Domain.Operations.Types.HexColor;

internal static class HexColorOperationExtensions
{
    internal static HexColorOperation AsHexColorOperation(
        this Operation operation,
        ILogger<IOperation> logger)
    {
        var configuration = JsonSerializer
            .Deserialize<HexColorOperationConfiguration>(operation.Configuration)!;

        return new HexColorOperation(operation, configuration, logger);
    }
}
