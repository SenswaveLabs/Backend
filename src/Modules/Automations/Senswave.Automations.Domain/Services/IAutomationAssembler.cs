using Senswave.Automations.Domain.Entities;

namespace Senswave.Automations.Domain.Services;

public interface IAutomationAssembler
{
    public Task AssembleResults(Automation automation);
}