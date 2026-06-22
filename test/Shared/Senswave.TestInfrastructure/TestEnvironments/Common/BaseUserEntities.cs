namespace Senswave.TestInfrastructure.TestEnvironments.Common;

public struct BaseUserEntities
{
    public Guid HomeId { get; set; }
    public Guid BrokerId { get; set; }

    public Guid DeviceId { get; set; }
    public Guid OperationId1 { get; set; }
    public Guid OperationId2 { get; set; }

    public Guid AutomationId { get; set; }
}