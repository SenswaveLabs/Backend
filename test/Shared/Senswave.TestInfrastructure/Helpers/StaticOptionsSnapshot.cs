using Microsoft.Extensions.Options;

namespace Senswave.TestInfrastructure.Extensions;

public static partial class TestEnvironmentExtensions
{
    public class StaticOptionsSnapshot<T>(T value) : IOptionsSnapshot<T> where T : class, new()
    {
        public T Value => value;
        public T Get(string? name) => value;
    }
}
