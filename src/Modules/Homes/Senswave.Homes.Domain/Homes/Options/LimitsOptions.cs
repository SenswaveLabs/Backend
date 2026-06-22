namespace Senswave.Homes.Domain.Homes.Options;

public class LimitsOptions
{
    public const string SectionName = $"{HomeModuleOptions.SectionName}:Limits";

    public int Homes { get; set; } = 4;
    public int RoomsPerHome { get; set; } = 16;
    public int UsersPerHome { get; set; } = 10;
}
