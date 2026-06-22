using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Devices.Domain.Operations.Types;
using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Enums;
using Senswave.Devices.Domain.Widgets.Extensions;
using Senswave.Devices.Domain.Widgets.Models;

namespace Senswave.Devices.Domain.Widgets.Types.Empty;

public class EmptyWidget : IWidget
{
    public IOperation Operation => throw new InvalidOperationException("Cannot access Operation field of empty widget.");

    public List<OperationType> AllowedOperationTypes => [];

    public Result<JsonValue> PreprocessAction(JsonValue incomingValue) => Result<JsonValue>.Failure(Error.Failure("OperationNotImplemented", "This operation is not implemented for empty widgets."));

    public Task<DisplayWidgetModel> ToDisplay() => Task.FromResult<DisplayWidgetModel>(new()
    {
        Id = Guid.Empty,

        Name = "Empty Widget",
        Type = WidgetType.Empty.FromWidgetType(),
        Enabled = false,

        Configuration = [],
        CreatedAtUtc = DateTime.UtcNow,
        UpdatedAtUtc = DateTime.UtcNow
    });

    public Widget AsWidget() => new();

    public Task<Result> Validate() => Task.FromResult(Result.Failure(Error.Failure("EmptyWidget", "Widget is empty and cannot be validated.")));
}
