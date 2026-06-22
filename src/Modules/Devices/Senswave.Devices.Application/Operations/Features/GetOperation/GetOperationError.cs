namespace Senswave.Devices.Application.Operations.Features.GetOperation;

public static class GetOperationError
{
    public static Error OperationNotFound => Error.NotFound("OperationNotFound", "Operation not found");

    public static Error TopicNotFound => Error.NotFound("TopicNotFound", "Topic not found");
}