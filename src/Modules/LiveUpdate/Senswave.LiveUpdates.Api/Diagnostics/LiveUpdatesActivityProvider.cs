using Senswave.Abstractions.Diagnostics;

namespace Senswave.LiveUpdates.Api.Diagnostics;

internal sealed class LiveUpdatesActivityProvider : ActivityProvider, ILiveUpdatesActivityProvider
{
    public LiveUpdatesActivityProvider() : base(LiveUpdatesModule.DefaultListenerName)
    {
    }
}
