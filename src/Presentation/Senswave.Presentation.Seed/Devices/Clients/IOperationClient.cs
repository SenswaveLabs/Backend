using Refit;
using Senswave.Devices.Api.Operations.CreateOperation;

namespace Senswave.Presentation.Seed.Devices.Clients;

public interface IOperationClient
{
    [Post("/v1/devices/operations")]
    Task<OperationCreatedResponse> CreateOperation([Authorize(scheme: "Bearer")] string token, CreateOperationRequest home);
}
