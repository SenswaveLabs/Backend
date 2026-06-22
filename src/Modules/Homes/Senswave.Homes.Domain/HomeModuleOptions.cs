using Senswave.Homes.Domain.Homes.Options;
using Senswave.Homes.Domain.Sharings.Options;

namespace Senswave.Homes.Domain;

public class HomeModuleOptions
{
    public const string SectionName = "Modules:Homes";

    public LimitsOptions Limits { get; set; } = new();

    public SharingsOptions Sharings { get; set; } = new();
}
