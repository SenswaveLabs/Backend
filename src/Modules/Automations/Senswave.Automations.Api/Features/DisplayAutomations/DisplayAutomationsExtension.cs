using Senswave.Automations.Application.Models;

namespace Senswave.Automations.Api.Features.DisplayAutomations;

internal static class DisplayAutomationsExtension
{
    public static DisplayAutomationsResponse ToResponse(this IEnumerable<AutomationModel> models) => new()
    {
        Items = models.ToList()
    };
}