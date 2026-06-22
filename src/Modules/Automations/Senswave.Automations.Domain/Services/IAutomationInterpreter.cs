using Senswave.Automations.Domain.Entities;

namespace Senswave.Automations.Domain.Services;

public interface IAutomationInterpreter
{
    public Task<bool> Interpret(Automation automation, CancellationToken cancellationToken);
}