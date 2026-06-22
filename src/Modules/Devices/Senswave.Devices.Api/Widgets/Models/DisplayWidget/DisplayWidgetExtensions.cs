using Senswave.Abstractions.Resulting;
using Senswave.Devices.Domain.Widgets.Models;

namespace Senswave.Devices.Api.Widgets.Models.DisplayWidget;

internal static class DisplayWidgetExtensions
{
    internal static List<DisplayWidgetDto> ToDisplayWidget(this Result<List<DisplayWidgetModel>> result) => [.. result.Data.Select(x => x.ToDto())];

    internal static DisplayWidgetDto ToDto(this Result<DisplayWidgetModel> result) => result.Data.ToDto();

    internal static DisplayWidgetDto ToDto(this DisplayWidgetModel model) => new()
    {
        Id = model.Id,

        Name = model.Name,
        Type = model.Type,
        Enabled = model.Enabled,
        Display = model.Configuration,

        UpdatedAtUtc = model.UpdatedAtUtc,
        CreatedAtUtc = model.CreatedAtUtc
    };
}
