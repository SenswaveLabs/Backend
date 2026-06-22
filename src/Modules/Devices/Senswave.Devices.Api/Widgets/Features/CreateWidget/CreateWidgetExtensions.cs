using Senswave.Abstractions.Resulting;
using Senswave.Devices.Application.Widgets.Features.CreateWidget;
using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Extensions;

namespace Senswave.Devices.Api.Widgets.Features.CreateWidget;

internal static class CreateWidgetExtensions
{
    internal static WidgetCreatedResponse ToWidgetCreatedResponse(this Result<Widget> results) => new()
    {
        Id = results.Data.Id
    };

    internal static CreateWidgetCommand ToCommand(this CreateWidgetRequest dto, Guid ownerId) => new()
    {
        UserId = ownerId,
        OperationId = dto.OperationId,
        Name = dto.Name,
        Configuration = dto.Configuration,
        Type = dto.Type.ToWidgetType()
    };
}