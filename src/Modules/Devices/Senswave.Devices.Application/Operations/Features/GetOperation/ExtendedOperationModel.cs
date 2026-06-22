using Senswave.Devices.Domain.Operations.Entities;

namespace Senswave.Devices.Application.Operations.Features.GetOperation;

public class ExtendedOperationModel
{
    public Operation Operation { get; set; } = new();
    public string Topic { get; set; } = string.Empty;
}