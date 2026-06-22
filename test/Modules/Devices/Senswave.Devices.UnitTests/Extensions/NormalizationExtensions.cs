using System.Runtime.InteropServices;

namespace Senswave.Devices.UnitTests.Extensions;

public static class NormalizationExtensions
{
    public static string NormalizeJson(this string json)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return json;

        return json.Replace("\r\n", Environment.NewLine);
    }
}
