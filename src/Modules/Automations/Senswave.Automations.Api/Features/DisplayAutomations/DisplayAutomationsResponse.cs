using Senswave.Automations.Application.Models;

namespace Senswave.Automations.Api.Features.DisplayAutomations;

public class DisplayAutomationsResponse
{
    public IList<AutomationModel> Items { get; set; } = [];
}