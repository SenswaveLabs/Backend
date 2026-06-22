using Refit;
using Senswave.Devices.Api.Widgets.Features.CreateWidget;

namespace Senswave.Presentation.Seed.Devices.Clients;

public interface IWidgetClient
{
    [Post("/v1/devices/widgets")]
    Task<WidgetCreatedResponse> CreateWidget([Authorize(scheme: "Bearer")] string token, CreateWidgetRequest home);
}
