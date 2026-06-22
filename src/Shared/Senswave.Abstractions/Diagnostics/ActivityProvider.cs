using System.Diagnostics;

namespace Senswave.Abstractions.Diagnostics;

public abstract class ActivityProvider(string name) : IActivityProvider
{

    protected readonly ActivitySource activitySource = new(name);

    public virtual Activity? StartActivity(string name, ActivityKind activityKind = ActivityKind.Internal) => activitySource.StartActivity(name, activityKind)!;
}
