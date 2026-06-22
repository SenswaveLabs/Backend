using Refit;
using Senswave.Automations.Api.Features.CreateAutomation;

namespace Senswave.Presentation.Seed.Automations.Clients;

public interface IAutomationClient
{
    [Post("/v1/automations")]
    Task<AutomationCreatedResponse> CreateAutomation([Authorize(scheme: "Bearer")] string token, CreateAutomationRequest createAutomationRequest);
}