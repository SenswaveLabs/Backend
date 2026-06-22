using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Enums;
using System.Text.Json.Nodes;

namespace Senswave.Devices.Application.Operations.Features.CreateOperation;

public class CreateOperationCommand : ICommand<Operation>
{
    public Guid UserId { get; set; } = Guid.Empty;
    public Guid DeviceId { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;

    public JsonObject Configuration { get; set; } = [];
    public OperationType Type { get; set; }
}
