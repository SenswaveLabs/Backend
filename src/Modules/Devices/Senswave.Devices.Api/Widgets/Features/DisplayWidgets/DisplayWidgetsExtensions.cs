using Senswave.Abstractions.Resulting;
using Senswave.Devices.Application.Widgets.Features.DisplayWidgets;
using Senswave.Devices.Domain.Operations.Extensions;
using Senswave.Devices.Domain.Widgets.Extensions;

namespace Senswave.Devices.Api.Widgets.Features.DisplayWidgets;

internal static class DisplayWidgetsExtensions
{
    internal static DisplayWidgetsResponse ToResponse(this Result<List<DisplayWidgetsGroupModel>> result) => new()
    {
        Items = [.. result.Data.Select(x => x.ToDto())]
    };

    internal static DisplayGroupDto ToDto(this DisplayWidgetsGroupModel model) => new()
    {
        Operation = new()
        {
            Id = model.Operation.Id.ToString(),
            Name = model.Operation.Name,
            Type = model.Operation.Type.FromOperationType()
        },
        Widgets = [.. model.Widgets.Select(x => new WidgetDto
        {
            Id = x.Id.ToString(),
            Name = x.Name,
            Enabled = x.Enabled,
            Type = x.Type.FromWidgetType()
        })]
    };
}
