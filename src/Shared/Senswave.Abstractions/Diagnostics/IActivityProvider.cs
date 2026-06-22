using System.Diagnostics;

namespace Senswave.Abstractions.Diagnostics;

public interface IActivityProvider
{
    Activity? StartActivity(string name, ActivityKind activityKind = ActivityKind.Internal);
}
