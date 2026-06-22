namespace Senswave.Devices.Domain.Devices.Options;

public class LimitsOptions
{
    public const string SectionName = $"{DevicesOptions.SectionName}:Limits";

    public int DevicesPerHome { get; set; } = 30;
    public int OperationsPerDevice { get; set; } = 30;
    public int OperationValuesPerOperation { get; set; } = 100;
    public int DashboardsPerDevice { get; set; } = 4;
}
