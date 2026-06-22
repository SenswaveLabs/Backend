using Senswave.Abstractions.Diagnostics;

namespace Senswave.Infrastructure.Diagnostics;

internal sealed class InfrastructureDiagnosticsActivityProvider : ActivityProvider, IInfrastructureDiagnosticsActivityProvider
{
    public InfrastructureDiagnosticsActivityProvider() : base(InfrastructureModule.DefaultListenerName)
    {

    }
}
