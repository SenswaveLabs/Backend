using Senswave.Devices.Domain.Operations.Entities;

using System.Text.Json;

namespace Senswave.Devices.Domain.Operations.Types.Text;

internal static class TextOperationExtensions
{
    internal static TextOperation AsTextOperation(
        this Operation operation,
        ILogger<IOperation> logger)
    {
        var configuration = JsonSerializer
            .Deserialize<TextOperationConfiguration>(operation.Configuration)!;

        return new TextOperation(operation, configuration, logger);
    }
}
