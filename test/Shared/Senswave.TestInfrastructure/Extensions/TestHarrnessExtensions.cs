using MassTransit.Testing;
using Senswave.Integration.DataTransfer.Devices;

namespace Senswave.TestInfrastructure.Extensions;

public static class TestHarrnessExtensions
{
    public static bool IsMessageInSystem<T>(this ITestHarness testHarrness) =>
        testHarrness.Sent.Select<WidgetActionEvent>().Any() ||
        testHarrness.Published.Select<WidgetActionEvent>().Any() ||
        testHarrness.Consumed.Select<WidgetActionEvent>().Any();
}
