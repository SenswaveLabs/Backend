using Senswave.Abstractions.Diagnostics;

namespace Senswave.DataSources.Domain.Diagnostics;

public sealed class DataSourcesActivityProvider : ActivityProvider, IDataSourcesActivityProvider
{
    public const string DefaultListenerName = "Senswave.DataSources";

    public DataSourcesActivityProvider() : base(DefaultListenerName)
    {
    }
}
