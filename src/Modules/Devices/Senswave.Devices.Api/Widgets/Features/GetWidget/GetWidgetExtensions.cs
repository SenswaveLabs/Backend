using Senswave.Abstractions.Resulting;
using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Extensions;

namespace Senswave.Devices.Api.Widgets.Features.GetWidget;

internal static class GetWidgetExtensions
{
    public static GetWidgetResponse ToResponse(this Result<Widget> result) => new()
    {
        Id = result.Data.Id,

        DeviceId = result.Data.Operation!.DeviceId,
        OperationId = result.Data.OperationId,

        Name = result.Data.Name,
        Type = result.Data.Type.FromWidgetType(),
        Configuration = result.Data.Configuration,
        Enabled = result.Data.Enabled,
    };
}
